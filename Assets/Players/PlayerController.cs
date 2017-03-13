using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public bool IsDebug = false;

    public Team Team;
    public AudioSource SFXPush;
    
    private const int CastMask = 1 << Layers.Solid | 1 << Layers.IgnoreColumnSupport;
    private const float CastRadius = 0.2f;

    private const float ActionDelay = BlockColumnManager.SlideBlockDuration * 2;
    private const float MaxHorizontalSpeed = 4.0f;
    private const float DistanceToGround = 0.5f;
    private const float InitialJumpVerticalSpeed = 5.0f;
    private const float JumpHorizontalAcceleration = 15;
    private const float AxisOnThreshold = 0.5f;
    
    private bool _canPerformAction;
    
    private Rigidbody _rigidbody;
    private Animator _anim;

    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _anim = GetComponentInChildren<Animator>();
        var audioSources = GetComponents<AudioSource>();
        SFXPush = audioSources[0];
        Initialize();
    }

    void FixedUpdate()
    {
        CheckIfJumpEnds();
    }

    private void CheckIfJumpEnds()
    {
        if (_anim.GetBool(AnimationParameters.IsJumpingMidair) && _rigidbody.velocity.y < 0)
        {
            var endJumpAnimTime = AnimationUtility.AnimationClipWithName(_anim, AnimationParameters.JumpLandingName).length;
            if (!IsOpenForMove(Vector3.down, Mathf.Abs(_rigidbody.velocity.y * endJumpAnimTime)))
            {
                _anim.SetBool(AnimationParameters.IsJumpingMidair, false);
            }
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

    public void Move(float horizontalAxis, float verticalAxis)
    {
        var velocity = _rigidbody.velocity;
        var isMovingHorizontally = Mathf.Abs(horizontalAxis) > AxisOnThreshold;
        var isMovingVertically = Mathf.Abs(verticalAxis) > AxisOnThreshold;
        if (IsGrounded())
        {
            var isWalking = isMovingHorizontally || isMovingVertically;
            if (isWalking)
            {
                var horizontalDirection = horizontalAxis > 0 ? Vector3.right : Vector3.left;
                var verticalDirection = verticalAxis > 0 ? Vector3.forward : Vector3.back;
                var dominantDirection = Mathf.Abs(horizontalAxis) > Mathf.Abs(verticalAxis)
                    ? horizontalDirection
                    : verticalDirection;

                transform.rotation = Quaternion.LookRotation(dominantDirection);

                velocity.x = dominantDirection.x * MaxHorizontalSpeed;
            }
            else
            {
                velocity.x = 0;
            }

            _anim.SetBool(AnimationParameters.IsWalking, isWalking);
        }
        else
        {
            if (isMovingHorizontally)
            {
                transform.rotation = Quaternion.LookRotation(Vector3.right * Mathf.Sign(horizontalAxis));

                velocity.x += Mathf.Sign(horizontalAxis) * JumpHorizontalAcceleration * Time.deltaTime;
                if (Mathf.Abs(velocity.x) > MaxHorizontalSpeed)
                {
                    velocity.x = Mathf.Sign(velocity.x) * MaxHorizontalSpeed;
                }
            }
            else
            {
                var oldSign = Mathf.Sign(velocity.x);
                velocity.x -= oldSign * JumpHorizontalAcceleration * Time.deltaTime;
                var isHorizontalDirectionChanged = !Mathf.Approximately(oldSign, Mathf.Sign(velocity.x));
                if (isHorizontalDirectionChanged)
                {
                    velocity.x = 0;
                }
            }
        }
        
        _rigidbody.velocity = velocity;
    }

    public void Jump()
    {
        if(_canPerformAction && IsGrounded())
        {
            _rigidbody.AddForce(Vector3.up * InitialJumpVerticalSpeed, ForceMode.Impulse);
            _anim.SetBool(AnimationParameters.IsJumping, true);
            _anim.SetBool(AnimationParameters.IsJumpingMidair, true);
            StartCoroutine(ActionDelayCoroutine());
        }
    }
    
    public void TryPushBlock()
    {
        if (_canPerformAction && !IsOpen(transform.position + transform.forward) && IsGrounded())
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
        if (_canPerformAction && !IsOpen(transform.position + transform.forward) && IsGrounded())
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

    private bool IsGrounded()
    {
        return !IsOpen(transform.position - new Vector3(0.0f, DistanceToGround, 0.0f));
    }

    private bool IsOpenForMove(Vector3 direction, float distance)
    {
        RaycastHit hitInfo;
        var capsuleCollider = GetComponent<CapsuleCollider>();

        var boxDimensions = new Vector3(capsuleCollider.radius, capsuleCollider.height/2 * 0.9f, capsuleCollider.radius);
        boxDimensions.Scale(transform.localScale);

        var hit =  Physics.BoxCast(
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
        yield return new WaitForSeconds(ActionDelay);
        _canPerformAction = true;
    }
}
