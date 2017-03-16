using System;
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

    private Vector3 _pushingDirection;

    private bool _canPerformAction;
    private bool _canMoveWhenPush;
    private bool _isPulling;

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

    void Update()
    {
        CheckIfFallingFar();
    }

    public void Initialize()
    {
        _canPerformAction = true;
        _canMoveWhenPush = true;
        _isPulling = false;
        _rigidbody.useGravity = true;
        GetComponent<Rigidbody>().velocity = Vector3.zero;
    }

    public bool IsFacing(Vector3 direction)
    {
        return transform.forward == direction;
    }

    public void Move(float horizontalAxis, float verticalAxis)
    {
        if (_isPulling) {
            return;
        }

        var velocity = _rigidbody.velocity;
        var isMovingHorizontally = Mathf.Abs (horizontalAxis) > AxisOnThreshold;
        var isMovingVertically = Mathf.Abs (verticalAxis) > AxisOnThreshold;
        if (IsGrounded ()) {
            var isWalking = isMovingHorizontally || isMovingVertically;
            if (isWalking) {
                var horizontalDirection = horizontalAxis > 0 ? Vector3.right : Vector3.left;
                var verticalDirection = verticalAxis > 0 ? Vector3.forward : Vector3.back;
                var dominantDirection = Mathf.Abs (horizontalAxis) > Mathf.Abs (verticalAxis)
            ? horizontalDirection
            : verticalDirection;

                transform.rotation = Quaternion.LookRotation (dominantDirection);

                if (!_canMoveWhenPush && horizontalDirection == _pushingDirection) {
                    velocity.x = 0;
                } else {
                    velocity.x = dominantDirection.x * MaxHorizontalSpeed;
                }
            } else {
                velocity.x = 0;
            }

            _anim.SetBool (AnimationParameters.IsWalking, isWalking);
        } else {
            if (isMovingHorizontally) {
                transform.rotation = Quaternion.LookRotation (Vector3.right * Mathf.Sign (horizontalAxis));

                velocity.x += Mathf.Sign (horizontalAxis) * JumpHorizontalAcceleration * Time.deltaTime;
                if (Mathf.Abs (velocity.x) > MaxHorizontalSpeed) {
                    velocity.x = Mathf.Sign (velocity.x) * MaxHorizontalSpeed;
                }
            } else {
                var oldSign = Mathf.Sign (velocity.x);
                velocity.x -= oldSign * JumpHorizontalAcceleration * Time.deltaTime;
                var isHorizontalDirectionChanged = !Mathf.Approximately (oldSign, Mathf.Sign (velocity.x));
                if (isHorizontalDirectionChanged) {
                    velocity.x = 0;
                }
            }
        }

        _rigidbody.velocity = velocity;
    }

    public void Jump()
    {
        if(_canPerformAction &&
            !_isPulling &&
            IsGrounded())
        {
            _rigidbody.AddForce(Vector3.up * InitialJumpVerticalSpeed, ForceMode.Impulse);
            _anim.SetBool(AnimationParameters.TriggerJumping, true);
            _anim.SetBool(AnimationParameters.IsJumpingMidair, true);
            StartCoroutine(ActionDelayCoroutine());
        }
    }

    public void TryMoveBlock(float horizontalAxis, float verticalAxis)
    {
        if (Mathf.Abs(horizontalAxis) > 0 || Mathf.Abs(verticalAxis) > 0)
        {
            var horizontalDirection = horizontalAxis > 0 ? Vector3.right : Vector3.left;
            var verticalDirection = verticalAxis > 0 ? Vector3.forward : Vector3.back;
            var dominantDirection = Mathf.Abs(horizontalAxis) > Mathf.Abs(verticalAxis)
                ? horizontalDirection
                : verticalDirection;

            if (IsFacing(dominantDirection))
            {
                TryPushBlock();
            }
            else if (IsFacing(-dominantDirection))
            {
                TryPullBlock();
            }
        }
    }
    
    public void TryPushBlock()
    {
        if (_canPerformAction &&
            !_isPulling &&
            !IsOpen(transform.position + transform.forward) && IsGrounded())
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
                StartCoroutine(PushBlockCoroutine(block));
            }
        }
    }

    public void TryPullBlock()
    {
        if (_canPerformAction &&
            !_isPulling &&
            !IsOpen(transform.position + transform.forward) && IsGrounded())
        {
            var block = GetBlockInFront();
            var direction = -transform.forward;

            if (!block.GetComponent<Block>().IsLocked)
            {
                StartCoroutine(PullBlockCoroutine(block, direction));
            }
        }
    }

    private void CheckIfJumpEnds()
    {
        if (_anim.GetBool(AnimationParameters.IsJumpingMidair) && _rigidbody.velocity.y < 0) {

            float endJumpAnimTime = AnimationUtility.AnimationClipWithName(_anim, AnimationParameters.JumpLandingName).length;
            if (!IsOpenForMove(Vector3.down, Mathf.Abs(_rigidbody.velocity.y * endJumpAnimTime))) {
                _anim.SetBool(AnimationParameters.IsJumpingMidair, false);
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

    private void CheckIfFallingFar()
    {
        if (_rigidbody.velocity.y < -1.0f && !_anim.GetBool(AnimationParameters.IsFallingFar))
        {
            var zIndex = transform.position.z;
            var blueTeamZIndexThreshold = -1.7;
            var purpleTeamZIndexThreshold = 1.7;

            if ((Team == Team.Blue && zIndex < blueTeamZIndexThreshold) ||
                (Team == Team.Purple && zIndex > purpleTeamZIndexThreshold))
            {
                _anim.SetBool(AnimationParameters.IsFallingFar, true);
            }
        }
    }

    private IEnumerator ActionDelayCoroutine()
    {
        _canPerformAction = false;
        yield return new WaitForSeconds(ActionDelay);
        _canPerformAction = true;
    }

    private IEnumerator PushBlockCoroutine(GameObject block)
    {
        if (Physics.Raycast (block.transform.position, Vector3.up, 1)) {
            _canMoveWhenPush = false;
            if (block.transform.position.x - transform.position.x > 0.5) {
                _pushingDirection = Vector3.right;
            } else {
                _pushingDirection = Vector3.left;
            }
        }
        _anim.SetBool(AnimationParameters.TriggerPushing, true);
        StartCoroutine(ActionDelayCoroutine());

        // TODO: When Pushing animation is split into 2, we can remove this delay
        yield return new WaitForSeconds(0.1f);

        var direction = transform.forward;
        BlockColumnManager.Instance.SlideBlock(block, direction);
        SFXPush.Play();
        yield return new WaitForSeconds (1f);
        _canMoveWhenPush = true;
    }

    private IEnumerator PullBlockCoroutine(GameObject block, Vector3 direction)
    {
        StartCoroutine(ActionDelayCoroutine());
        _isPulling = true;

        _anim.SetBool(AnimationParameters.TriggerPulling, true);
        _anim.SetBool (AnimationParameters.IsWalking, false);

        // Wait for pulling animation to finish
        while(!(_anim.IsInTransition(0) && _anim.GetNextAnimatorStateInfo(0).IsName(AnimationParameters.ClimbingName)))
        {
            yield return null;
        }

        BlockColumnManager.Instance.SlideBlock(block, direction);
        SFXPush.Play();

        // Climbing animation is now playing
        var oldPosition = transform.position;
        var animationTime =  0.10f;
        var climbDelta = Vector3.up;

        // yield for the player animation to perform
        // the animation of placing their hands ontop of the block
        yield return new WaitForSeconds(0.05f);

        // Move upwards
        _rigidbody.useGravity = false;
        StartCoroutine(AxisMoveCoroutine(transform, Vector3.up, 1, animationTime));

        yield return new WaitForSeconds(animationTime);

        // Move towards pulled block
        _rigidbody.useGravity = true;

        if (IsOpenForMove(direction * -1,  Mathf.Abs(transform.position.z - oldPosition.z)))
        {
            var newPosition = transform.position;
            newPosition.z = oldPosition.z;
            transform.position = newPosition;
        }
        else
        {
            // Cause player to die, by adding a force towards a block
            _rigidbody.AddForce(direction * -1f, ForceMode.Impulse);
        }

        _rigidbody.velocity = Vector3.zero;
        _isPulling = false;
    }

    private IEnumerator AxisMoveCoroutine(Transform moveTransform, Vector3 axis, int distance, float animationTime, Action completion = null) {

        var elapsedTime = 0.0f;
        var oldPositionAtAxis = Vector3.Scale(moveTransform.position, axis);
        var distanceVector = axis * distance;
        var axisZeroVector = Vector3.one - axis;

        while (elapsedTime < animationTime) {

            var position = moveTransform.position;

            // Zero out the position at axis
            position = Vector3.Scale(position, axisZeroVector);

            moveTransform.position = position +
                Vector3.Lerp(oldPositionAtAxis,
                    oldPositionAtAxis + distanceVector,
                    elapsedTime / animationTime);
            yield return new WaitForEndOfFrame();

            elapsedTime += Time.deltaTime;
        }

        moveTransform.position = Vector3.Scale(moveTransform.position, axisZeroVector) + oldPositionAtAxis + distanceVector;

        if (completion != null)
        {
            completion();
        }
    }
}
