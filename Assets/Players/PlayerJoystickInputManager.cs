using UnityEngine;

public class PlayerJoystickInputManager : MonoBehaviour
{
    public const KeyCode PauseKeyCode = KeyCode.P;
    
    public int PlayerNumber;
    public bool InvertVerticalAxis;

    public KeyCode PushKeyCode;
    public KeyCode PullKeyCode;
    public KeyCode JumpKeyCode;

    private string _joystickPauseKey;
    private string _joystickPushKey;
    private string _joystickPullKey;
    private string _joystickJumpKey;

    private PlayerController _controller;

    void Start()
    {
        _joystickPauseKey = string.Format("joystick {0} button 7", PlayerNumber);
        _joystickPushKey = string.Format("joystick {0} button 2", PlayerNumber);
        _joystickPullKey = string.Format("joystick {0} button 3", PlayerNumber);
        _joystickJumpKey = string.Format("joystick {0} button 0", PlayerNumber);

        _controller = GetComponent<PlayerController>();
    }

    void Update()
    {
        if (Input.GetKeyDown(PauseKeyCode) || Input.GetKeyDown(_joystickPauseKey))
        {
            GameController.Instance.TogglePause();
        }

        var leftHorizontalAxis = Input.GetAxis("L_XAxis_" + PlayerNumber);
        var leftVerticalAxis = Input.GetAxis("L_YAxis_" + PlayerNumber) * (InvertVerticalAxis ? -1 : 1);
        _controller.Move(leftHorizontalAxis, leftVerticalAxis);

        var rightHorizontalAxis = Input.GetAxis("R_XAxis_" + PlayerNumber);
        var rightVerticalAxis = Input.GetAxis("R_YAxis_" + PlayerNumber) * (InvertVerticalAxis ? -1 : 1);
        _controller.TryMoveBlock(rightHorizontalAxis, rightVerticalAxis);

        if (Input.GetKeyDown(_joystickPushKey) || Input.GetKeyDown(PushKeyCode))
        {
            _controller.TryPushBlock();
        }

        if (Input.GetKeyDown(_joystickPullKey) || Input.GetKeyDown(PullKeyCode))
        {
            _controller.TryPullBlock();
        }

        if (Input.GetKeyDown(_joystickJumpKey) || Input.GetKeyDown(JumpKeyCode))
        {
            _controller.Jump(); 
        }
    }
}