using UnityEngine;

public class PlayerJoystickInputManager : MonoBehaviour
{
    public int PlayerNumber;
    public bool InvertVerticalAxis;

    public KeyCode PushKeyCode;
    public KeyCode PullKeyCode;
    public KeyCode JumpKeyCode;

    private string _joystickPushKey;
    private string _joystickPullKey;
    private string _joystickJumpKey;

    private PlayerController _controller;

    void Start()
    {
        _joystickPushKey = string.Format("joystick {0} button 2", PlayerNumber);
        _joystickPullKey = string.Format("joystick {0} button 3", PlayerNumber);
        _joystickJumpKey = string.Format("joystick {0} button 0", PlayerNumber);

        _controller = GetComponent<PlayerController>();
    }

    void Update()
    {
        var horizontalAxis = Input.GetAxis("Horizontal_P" + PlayerNumber);
        var verticalAxis = Input.GetAxis("Vertical_P" + PlayerNumber) * (InvertVerticalAxis ? -1 : 1);
        _controller.Move(horizontalAxis, verticalAxis);
        
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