using System.Collections;
using System.Linq;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private const int CastMask = 1 << Layers.Solid;
    private const float CastRadius = 0.1f;
    private const float MoveDurationInSeconds = 0.25f;

    private bool _isMoving;
    private float _moveTimer;
    private GameObject _grabbedBlock;


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

    public void Move(Vector3 direction)
    {
        if (_isMoving)
        {
            return;
        }

        var canMovePlayerInDirection = IsOpen(transform.position + direction);

        if (_grabbedBlock != null)
        {
            var canMoveGrabbedBlockInDirection = IsOpen(_grabbedBlock.transform.position + direction);
            if (canMovePlayerInDirection && canMoveGrabbedBlockInDirection)
            {
                StartCoroutine(MoveCoroutine(new [] { transform, _grabbedBlock.transform }, direction));
            }
        }
        else
        {
            transform.rotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(direction, Vector3.up));

            if (canMovePlayerInDirection)
            {
                StartCoroutine(MoveCoroutine(new [] {transform}, direction));
            }
        }
    }

    public void Jump()
    {
        if (_isMoving || _grabbedBlock != null)
        {
            return;
        }

        if (!IsOpen(transform.position + transform.forward) &&
            IsOpen(transform.position + transform.forward + Vector3.up))
        {
            StartCoroutine(MoveCoroutine(new[] {transform}, transform.forward + Vector3.up));
        }
    }

    public void GrabBlock()
    {
        var position = transform.position + transform.forward;
        var colliders = Physics.OverlapSphere(position, CastRadius, CastMask);
        if (colliders.Length == 1)
        {
            _grabbedBlock = colliders[0].gameObject;
        }
        else if (colliders.Length > 1)
        {
            Debug.LogError(string.Format("There are {0} blocks to grab at {1}.", colliders.Length, position));
        }
    }

    public void UngrabBlock()
    {
        _grabbedBlock = null;
    }

    private bool IsOpen(Vector3 position)
    {
        var colliders = Physics.OverlapSphere(position, CastRadius, CastMask);
        if (colliders.Length == 1 && colliders[0].gameObject == _grabbedBlock)
        {
            return true;
        }

        return colliders.Length == 0;
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
