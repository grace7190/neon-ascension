using System.Collections;
using System.Linq;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public GameObject Wall;

    public const float GravityScale = 10.0f;
    private const int CastMask = 1 << Layers.Solid;
    private const float CastRadius = 0.1f;
    private const float MoveDurationInSeconds = 0.25f;

    private bool _isMoving;
    private bool _isFalling;
    private float _moveTimer;
    

    void Update()
    {
        if (!_isMoving)
        {
            transform.position = new Vector3(Mathf.RoundToInt(transform.position.x), transform.position.y,
                Mathf.RoundToInt(transform.position.z));
        }
    }

    void FixedUpdate()
    {
        var gravity = Physics.gravity * GravityScale;
        GetComponent<Rigidbody>().AddForce(gravity, ForceMode.Acceleration);
    }

    void OnTriggerEnter(Collider other) {
        var collidingObject = other.gameObject;
        if (collidingObject.CompareTag(Tags.Block)) {
            if (gameObject.transform.position.x - collidingObject.transform.position.x < 0.5
                && gameObject.transform.position.y < collidingObject.transform.position.y) {
                Destroy (gameObject);
            }
        }
    }

    void OnCollisionEnter(Collision collision) {
        if (collision.transform.position.y < transform.position.y)
        {
            _isFalling = false;
        }
    }

    public void Move(Vector3 direction)
    {
        if (_isMoving || _isFalling)
        {
            return;
        }

        transform.rotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(direction, Vector3.up));

        var canPlayerMoveInDirection = IsOpen(transform.position + direction);
        var canPlayerJumpInDirection = IsOpen(transform.position + direction + Vector3.up);

        if (canPlayerMoveInDirection)
        {
            // Check if player is jumping down, check for the platform under direction
            var isPlayerJumpingDownInDirection = IsOpen(transform.position + direction + Vector3.down);

            if (isPlayerJumpingDownInDirection) {
                // Lock movement till player has reached the bottom
                _isFalling = true;
            }

            StartCoroutine(MoveCoroutine(new[] { transform }, direction));
        }
        else if (canPlayerJumpInDirection)
        {
            StartCoroutine(MoveCoroutine(new[] { transform }, direction + Vector3.up));
        }
    }

    public void TryPushBlock()
    {
        if (!IsOpen(transform.position + transform.forward) &&
            IsOpen(transform.position + transform.forward * 2) &&
            !_isFalling)
        {
            var block = GetBlockInFront();
            var direction = transform.forward;

            if (!block.GetComponent<Block>().IsLocked)
            {
                BlockColumnManager.Instance.SlideBlock(block, direction);
            }
        }
    }

    public void TryPullBlock()
    {
        if (!IsOpen(transform.position + transform.forward) && !_isFalling)
        {
            var block = GetBlockInFront();
            var direction = -transform.forward;

            if (!block.GetComponent<Block>().IsLocked)
            {
                BlockColumnManager.Instance.SlideBlock(block, direction);
                StartCoroutine(MoveCoroutine(new[] {transform}, Vector3.up));
            }
        }
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
