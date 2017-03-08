using System.Collections;
using System.Linq;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public const float GravityScale = 10.0f;
    public bool IsDebug = false;
    public bool IsFalling { get; private set; }

    public Team Team;

    private const int CastMask = 1 << Layers.Solid | 1 << Layers.IgnoreColumnSupport;
    private const float CastRadius = 0.2f;
    private const float MoveDurationInSeconds = 0.25f;
    private const float speed = 4.0f;
    private const float jumpVelocity = 18.0f;
    private const float groundCheck = 0.5f;
    private const float _actionDelay = BlockColumnManager.SlideBlockDuration * 2;
     
    private bool _isMoving;
    private float _moveTimer;
    private bool _canPerformAction;

    private Rigidbody _rb;
    private Animator _anim;
    public AudioSource SFXPush;

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _anim = GetComponentInChildren<Animator>();
        var audioSources = GetComponents<AudioSource>();
        SFXPush = audioSources[0];
        Initialize();
    }

    void FixedUpdate()
    {
        var gravity = Physics.gravity * GravityScale;
        GetComponent<Rigidbody>().AddForce(gravity, ForceMode.Acceleration);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.position.y < transform.position.y)
        {
            IsFalling = false;
        }
    }

    public void Initialize()
    {
        _canPerformAction = true;
        GetComponent<Rigidbody>().velocity = Vector3.zero;
    }

    public bool IsFacing(Vector3 direction)
    {
        var yAngle = transform.eulerAngles.y;
        var flip = 1;
        if (yAngle > 180)
        {
            yAngle -= 180;
            flip = -1;
        }
        var roundRotation = Quaternion.Euler(0.0f, (flip*yAngle / 90) * 90, 0.0f);
        return roundRotation != Quaternion.LookRotation(Vector3.ProjectOnPlane(direction, Vector3.up));

    }

    public bool Turn(Vector3 direction)
    {
        if (IsFacing(direction))
        {
            transform.rotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(direction, Vector3.up));
            return true;
        }
        return false;
    }

    public void Move(float hor, float vert)
    {
        float deltaMovement = (float)System.Math.Round(Time.deltaTime*speed,1);
        Vector3 newPosition = transform.position + new Vector3(hor*deltaMovement, 0, 0);

        if (IsOpenForMove(new Vector3(hor, 0, 0), deltaMovement)) {
            transform.position = newPosition;
        }
        
    }

    public void Jump()
    {
        if(isGrounded())
        {
            _rb.velocity = new Vector3(0.0f, jumpVelocity, 0.0f);
        }
    }
    
    public void TryPushBlock()
    {
        if (_canPerformAction && !IsOpen(transform.position + transform.forward) && isGrounded())
        {
            var block = GetBlockInFront();
            var isBlockBlocked = !IsOpen(transform.position + transform.forward * 2);

            if (block.GetComponent<Block>().IsLocked)
            {
                return;
            }

            if (isBlockBlocked)
            {
                block.GetComponent<Block>().AnimateBlocked();
            }
            else
            {
                var direction = transform.forward;
                BlockColumnManager.Instance.SlideBlock(block, direction);
                SFXPush.Play();

                StartCoroutine(ActionDelayCoroutine());
            }
        }
    }

    public void TryPullBlock()
    {
        if (_canPerformAction && !IsOpen(transform.position + transform.forward) && isGrounded())
        {
            var block = GetBlockInFront();
            var direction = -transform.forward;

            if (!block.GetComponent<Block>().IsLocked)
            {
                Jump();
                BlockColumnManager.Instance.SlideBlock(block, direction);
                SFXPush.Play();

                StartCoroutine(ActionDelayCoroutine());
            }
        }
    }

    private bool isGrounded()
    {
        return !IsOpen(transform.position - new Vector3(0.0f, groundCheck, 0.0f));
    }

    private bool IsOpenForMove(Vector3 direction, float distance)
    {
        RaycastHit hitInfo = new RaycastHit();
        var capsuleCollider = GetComponent<CapsuleCollider>();

        var boxDimensions = new Vector3(capsuleCollider.radius, capsuleCollider.height/2 * 0.9f, capsuleCollider.radius);
        boxDimensions.Scale(transform.localScale);

        bool hit =  Physics.BoxCast(
                        transform.position,
                        boxDimensions,
                        direction,
                        out hitInfo,
                        Quaternion.identity,
                        distance,
                        CastMask,
                        QueryTriggerInteraction.Ignore);

        if (IsDebug)
        {
            if (hit)
            {
                hitInfo.collider.gameObject.GetComponent<Block>().AnimateBlocked();
            }

            ExtDebug.DrawBoxCastOnHit(
                transform.position,
                boxDimensions,
                Quaternion.identity,
                direction,
                distance,
                Color.blue);
        }

        return !hit;

    }

    private bool IsOpen(Vector3 position)
    {
        var colliders = Physics.OverlapSphere(position, CastRadius, CastMask);
        return colliders.Length == 0;
    }

    private GameObject GetBlockInFront()
    {
        return
            Physics.OverlapSphere(transform.position + transform.forward,
                                  CastRadius,
                                  CastMask,
                                  QueryTriggerInteraction.Ignore)[0].gameObject;
    }

    private IEnumerator ActionDelayCoroutine() {
        _canPerformAction = false;
        yield return new WaitForSeconds(_actionDelay);
        _canPerformAction = true;
    }


    private IEnumerator MoveCoroutine(Transform[] movedTransforms, Vector3 direction)
    {
        _isMoving = true;
        var oldPositions = movedTransforms.Select(i => i.position).ToList();
        _moveTimer = 0;

        while (_moveTimer < MoveDurationInSeconds)
        {
            yield return new WaitForEndOfFrame();
            _moveTimer += Time.deltaTime;
            for (var i = 0; i < movedTransforms.Length; i++)
            {
                movedTransforms[i].position = Vector3.Lerp(oldPositions[i], oldPositions[i] + direction,
                    Mathf.Pow(_moveTimer / MoveDurationInSeconds, 0.25f));
            }
        }

        for (var i = 0; i < movedTransforms.Length; i++)
        {
            movedTransforms[i].position = oldPositions[i] + direction;
        }

        _isMoving = false;
    }
}
