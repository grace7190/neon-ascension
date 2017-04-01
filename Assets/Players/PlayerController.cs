using System;
using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public bool IsDebug = false;

    // Let the respawner handle this
    public bool DisplayTutorialOnSpawn = true;

    public Team Team;

    public AudioSource SFXPush;
    public AudioSource SFXJump;
    public AudioSource SFXMove;

    private const int CastMask = 1 << Layers.Solid | 1 << Layers.IgnoreColumnSupport | 1 << Layers.FloorBlocks;
    private const float CastRadius = 0.2f;

    private const float ActionDelay = BlockColumnManager.SlideBlockDuration * 2;
    private const float MaxHorizontalSpeed = 4.0f;
    private const float DistanceToGround = 0.5f;
    private const float InitialJumpVerticalSpeed = 5.0f;
    private const float JumpHorizontalAcceleration = 15;
    private const float AxisOnThreshold = 0.5f;
    private const float StopIconDuration = 0.2f;

    private Vector3 _pushingDirection;

    private bool _canPerformAction;
    private bool _canMoveWhenPush;
    private bool _isPulling;

    private Rigidbody _rigidbody;
    private Animator _anim;
    private PlayerFacingBlockDetector _detector;
    private FeedbackIconManager _iconManager;
    private TutorialIconManager _tutorialManager;

    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _anim = GetComponentInChildren<Animator>();
        _detector =  GetComponentInChildren<PlayerFacingBlockDetector>();
        _iconManager = GetComponentInChildren<FeedbackIconManager>();
        _tutorialManager = GetComponent<TutorialIconManager>();

        AudioSource[] aSources = GetComponents<AudioSource>();

        SFXMove = aSources[0];
        SFXJump = aSources[1];
        SFXPush = aSources[2];

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
        _rigidbody.isKinematic = false;
        GetComponent<Rigidbody>().velocity = Vector3.zero;

        if (DisplayTutorialOnSpawn) {
            _tutorialManager.Reset();
        }
    }

    public void PerformDeathCleanup()
    {
        _detector.Reset();
        _iconManager.HideCurrentIcon();
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

        if (velocity.x != 0 && !_tutorialManager.TutorialDidFinish)
        {
            _tutorialManager.DidMove();
        }
    }

    public void Jump()
    {
        if(_canPerformAction &&
            !_isPulling &&
            IsGrounded())
        {
            SFXJump.Play();
            _rigidbody.AddForce(Vector3.up * InitialJumpVerticalSpeed, ForceMode.Impulse);
            _anim.Play(AnimationParameters.JumpTakeoffName);
            _anim.SetBool(AnimationParameters.IsJumpingMidair, true);
            StartCoroutine(ActionDelayCoroutine());

            if (!_tutorialManager.TutorialDidFinish)
            {
                _tutorialManager.DidJump();
            }
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

        var block = GetBlockInFront();

        if (_canPerformAction &&
            !_isPulling &&
            block != null &&
            IsGrounded())
        {
            var isBlockBlocked = !IsOpen(transform.position + transform.forward * 2);


            if (block.GetComponent<Block>().IsLocked)
            {
                return;
            }

            if (block.GetComponent<Block>().IsStatic ||
                isBlockBlocked)
            {

                if (!_anim.GetCurrentAnimatorStateInfo(0).IsName(AnimationParameters.PushingStartName) &&
                    !_anim.GetCurrentAnimatorStateInfo(0).IsName(AnimationParameters.PushingEndName)) {
                       _anim.Play(AnimationParameters.PushingStartName);
                    }

                _iconManager.ShowStopIcon(true, StopIconDuration);

                if (isBlockBlocked)
                {
                    block.GetComponent<Block>().AnimateBlocked();
                }

                return;
            }

            if (block.transform.position.z + 1 == BlockColumnManager.WallZIndex)
            {
                if (!_tutorialManager.TutorialDidFinish)
                {
                    _tutorialManager.DidPushWall();
                }

                ScoreManager.Instance.IncrementScoreForTeam(ScoreManager.PushWallScoreIncrement, Team);
            }

            StartCoroutine(PushBlockCoroutine(block));
        }
    }

    public void TryPullBlock()
    {
        var block = GetBlockInFront();

        if (_canPerformAction &&
            !_isPulling &&
            block != null &&
            IsGrounded())
        {
            var direction = -transform.forward;

            if (!block.GetComponent<Block>().IsLocked && !block.GetComponent<Block>().IsStatic)
            {
                if (!_tutorialManager.TutorialDidFinish)
                {
                    if (block.transform.position.z + 1 == BlockColumnManager.WallZIndex)
                    {
                        _tutorialManager.DidPullWall();
                    }
                    else
                    {
                        _tutorialManager.DidPullSideBlock();
                    }
                }

                StartCoroutine(PullBlockCoroutine(block, direction));
            }
            else if (block.GetComponent<Block>().IsStatic) {
                _iconManager.ShowStopIcon(true, StopIconDuration);
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
        var colliders = Physics.OverlapSphere(transform.position + transform.forward,
                                         CastRadius,
                                         CastMask,
                                         QueryTriggerInteraction.Ignore);

        // Try to select the collider of the gameobject that is highlighted
        // The gameobject that is highlighted may not have updated in time during a rotation of the player
        // That is why we must check what is in front and can not rely on the highlighted object
        foreach (var collider in colliders)
        {
            if (collider.gameObject == _detector.HighlightedObject)
            {
                return collider.gameObject;
            }
        }

        if (colliders.Length > 0)
        {
            return colliders[0].gameObject;
        }
        else
        {
            return null;
        }
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

        _anim.Play(AnimationParameters.PushingStartName);

        var direction = transform.forward;
        BlockColumnManager.Instance.SlideBlockWithEaseInAndLights(block, direction);
        SFXPush.Play();
        StartCoroutine(ActionDelayCoroutine());

        yield return new WaitForSeconds (1f);

        _canMoveWhenPush = true;
    }

    private IEnumerator PullBlockCoroutine(GameObject block, Vector3 direction)
    {
        StartCoroutine(ActionDelayCoroutine());
        _isPulling = true;

        _anim.Play(AnimationParameters.ClimbingName);
        _anim.SetBool (AnimationParameters.IsWalking, false);

        // Move upwards
        _rigidbody.useGravity = false;
        _rigidbody.isKinematic = true;

        BlockColumnManager.Instance.SlideBlockWithEaseInAndLights(block, direction);
        SFXPush.Play();

        // Climbing animation is now playing
        var oldPosition = transform.position;

        // DO NOT CHANGE the animationTime. It must be the same lenght as SlideBlockDuration or more
        // If any less, the player will be on the sliding block, while the block slides.
        // The friction between the player and the block will cause the player
        // to have a velocity on the z axis.
        var animationTime = BlockColumnManager.SlideBlockDuration - 0.05f;
        var climbDelta = Vector3.up;
        climbDelta.y += 0.1f;

        // yield for the player animation to perform
        // the animation of placing their hands ontop of the block
        yield return new WaitForSeconds(0.05f);

        StartCoroutine(AxisMoveCoroutine(transform, climbDelta, 1, animationTime));

        yield return new WaitForSeconds(animationTime);
        yield return new WaitForFixedUpdate();

        _rigidbody.useGravity = true;
        _rigidbody.isKinematic = false;

        if (IsOpenForMove(direction * -1,  Mathf.Abs(transform.position.z - oldPosition.z)))
        {
            var newPosition = transform.position;
            newPosition.z = oldPosition.z;
            newPosition.y = oldPosition.y + 1;
            transform.position = newPosition;
        }

        _rigidbody.velocity = Vector3.zero;
        _rigidbody.angularVelocity = Vector3.zero;

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
