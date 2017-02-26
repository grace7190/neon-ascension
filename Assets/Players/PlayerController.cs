﻿using System.Collections;
using System.Linq;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public const float GravityScale = 10.0f;

    public bool IsFalling { get; private set; }

    public Team Team;

    private const int CastMask = 1 << Layers.Solid;
    private const float CastRadius = 0.1f;
    private const float MoveDurationInSeconds = 0.25f;
    private const float speed = 4.0f;
    private const float jumpVelocity = 4.0f;
    private const float groundCheck = 0.5f;

    private bool _isMoving;
    private float _moveTimer;

    private Rigidbody rb; 

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        
    }

    void FixedUpdate()
    {
        var gravity = Physics.gravity * GravityScale;
        GetComponent<Rigidbody>().AddForce(gravity, ForceMode.Acceleration);
    }

    void OnCollisionEnter(Collision collision) {
        if (collision.transform.position.y < transform.position.y)
        {
            IsFalling = false;
        }
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
    
    public void Move(Vector3 direction)
    {
        if (_isMoving || IsFalling)
        {
            return;
        }

        var canPlayerMoveInDirection = IsOpen(transform.position + direction);
        var canPlayerJumpInDirection = IsOpen(transform.position + direction + Vector3.up);

        if (canPlayerMoveInDirection)
        {
            // Check if player is jumping down, check for the platform under direction
            var isPlayerJumpingDownInDirection = IsOpen(transform.position + direction + Vector3.down);

            if (isPlayerJumpingDownInDirection) {
                // Lock movement till player has reached the bottom
                IsFalling = true;
            }

            StartCoroutine(MoveCoroutine(new[] { transform }, direction));
        }
        else if (canPlayerJumpInDirection)
        {
            StartCoroutine(MoveCoroutine(new[] { transform }, direction + Vector3.up));
        }
    }

    public void Move2(float hor, float vert)
    {
        Vector3 newPosition = transform.position + new Vector3(hor*Time.deltaTime*speed, 0, 0);
        if (IsOpen(newPosition))
        {
            transform.position = newPosition;
        }
        
    }

    public void Jump()
    {
        if(isGrounded())
        {
            rb.velocity = new Vector3(0.0f, jumpVelocity, 0.0f);
        }
    }

    public void TryPushBlock()
    {
        if (!IsOpen(transform.position + transform.forward) &&
            IsOpen(transform.position + transform.forward * 2) &&
            isGrounded())
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
        if (!IsOpen(transform.position + transform.forward) && isGrounded())
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

    private bool isGrounded()
    {
        return !IsOpen(transform.position - new Vector3(0.0f, groundCheck, 0.0f));
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
