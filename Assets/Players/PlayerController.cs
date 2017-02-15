using System.Collections;
using System.Linq;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private const int CastMask = 1 << Layers.Solid;
    private const float CastRadius = 0.1f;
    private const float MoveDurationInSeconds = 0.25f;

    // The time in seconds that a column stays locked after a block was pulled
    // or pushed from it
    private const float ColumnLockDurationInSeconds = MoveDurationInSeconds + 0.5f;

    // The number of blocks in a column to lock from the bottom of the stack
    private const float ColumnLockDistance = 20;

    private bool _isMoving;
    private float _moveTimer;

    // Which position the column locking starts at. This value will probably change as the wall grows,
    // and the camera moves up
    public float columnLockStartY = -1;

    void Start()
    {
        Physics.IgnoreLayerCollision(Layers.Player, Layers.Hole);
    }

    void Update()
    {
        if (!_isMoving)
        {
            transform.position = new Vector3(Mathf.RoundToInt(transform.position.x), transform.position.y,
                Mathf.RoundToInt(transform.position.z));
        }
    }

	void OnTriggerEnter(Collider other) {
		GameObject collidingObject = other.gameObject;
		if (collidingObject.tag == "Block") {
			if (gameObject.transform.position.x - collidingObject.transform.position.x < 0.5
				&& gameObject.transform.position.y < collidingObject.transform.position.y) {
				Destroy (gameObject);
				Debug.Log (gameObject + " is destroyed.");
			}
		}
	}

    public void Move(Vector3 direction)
    {
        if (_isMoving)
        {
            return;
        }

        transform.rotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(direction, Vector3.up));


        var canPlayerMoveInDirection = IsOpen(transform.position + direction);
        var canPlayerJumpInDirection = IsOpen(transform.position + direction + Vector3.up);

        if (canPlayerMoveInDirection)
        {
            StartCoroutine(MoveCoroutine(new[] { transform }, direction));
        }
        else if (canPlayerJumpInDirection)
        {
            StartCoroutine(MoveCoroutine(new[] { transform }, direction + Vector3.up));
        }
    }

    public void TryPushBlock()
    {
        if (!IsOpen(transform.position + transform.forward) && IsOpen(transform.position + transform.forward * 2))
        {
            var block = GetBlockInFront();
            var moveable = block.gameObject.GetComponent<PlayerMoveable>();
            var direction = transform.forward;

            if (moveable.isLocked)
                return;

            moveable.finalDestination = block.transform.position + direction;
            LockColumnFromPosition(block.transform.position);
            StartCoroutine(MoveCoroutine(new[] { block.transform }, direction));
        }
    }

    public void TryPullBlock()
    {
        if (!IsOpen(transform.position + transform.forward))
        {
            var block = GetBlockInFront();
            var moveable = block.gameObject.GetComponent<PlayerMoveable>();
            var direction = -transform.forward;

            if (moveable.isLocked)
                return;

            moveable.finalDestination = block.transform.position + direction;
            LockColumnFromPosition(block.transform.position);
            StartCoroutine(MoveCoroutine(new[] {transform}, Vector3.up));
            StartCoroutine(MoveCoroutine(new[] {block.transform}, direction));
        }
    }

    private bool IsOpen(Vector3 position)
    {
        var colliders = Physics.OverlapSphere(position, CastRadius, CastMask);
        return colliders.Length == 0;
    }

    private Collider GetBlockInFront()
    {
        return
            Physics.OverlapSphere(transform.position + transform.forward,
                                  CastRadius,
                                  CastMask,
                                  QueryTriggerInteraction.Ignore)[0];
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

    /*
     * Given Vector3 position;
     * Lock blocks located from (position.x, columnLockStartY, position.z) to (position.x, ColumnLockDistance, position.z)
     * excluding the block at position.
     */
    private void LockColumnFromPosition(Vector3 position)
    {
        var positionAtBottom = new Vector3(position.x, columnLockStartY, position.z);
        var hits =
            Physics.RaycastAll(positionAtBottom,
                               Vector3.up,
                               ColumnLockDistance,
                               CastMask,
                               QueryTriggerInteraction.Ignore);

        for (int i = 0; i < hits.Length; i++)
        {
            var hit = hits[i];
            var gameObject = hit.rigidbody.gameObject;

            if (gameObject.tag == "Block" && gameObject.transform.position != position)
            {
                gameObject.GetComponent<PlayerMoveable>().SetLockedForDuration(ColumnLockDurationInSeconds);
            }
        }
    }
}
