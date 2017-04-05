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

    #if UNITY_STANDALONE_OSX
    // Reference: http://wiki.unity3d.com/index.php?title=Xbox360Controller
    private const string Button0 = "button 16";
    private const string Button2 = "button 18";
    private const string Button3 = "button 19";
    private const string Button7 = "button 9";

    private const string LXAxis = "L_XAxis_";
    private const string LYAxis = "L_YAxis_";

    private const string RXAxis = "R_XAxis_MAC_";
    private const string RYAxis = "R_XAxis_"; // Yes it's inverted

    private const string TriggerL = "TriggersL_MAC_";
    private const string TriggerR = "TriggersR_MAC_";

    #else
    private const string Button0 = "button 0";
    private const string Button7 = "button 7";

    private const string LXAxis = "L_XAxis_";
    private const string LYAxis = "L_YAxis_";

    private const string RXAxis = "R_XAxis_";
    private const string RYAxis = "R_YAxis_";

    private const string TriggerL = "TriggersL_";
    private const string TriggerR = "TriggersR_";
#endif

    void Start()
    {

        _joystickPauseKey = string.Format("joystick {0} " + Button7, PlayerNumber);
        _joystickPushKey = string.Format(TriggerL + "{0}", PlayerNumber);
        _joystickPullKey = string.Format(TriggerR + "{0}", PlayerNumber);
        _joystickJumpKey = string.Format("joystick {0} " + Button0, PlayerNumber);

        _controller = GetComponent<PlayerController>();
    }
    
    void Update()
    {
        if (Input.GetKeyDown(PauseKeyCode) || Input.GetKeyDown(_joystickPauseKey))
        {
            GameController.Instance.TogglePause();
        }

        var leftHorizontalAxis = Input.GetAxis(LXAxis + PlayerNumber);
        var leftVerticalAxis = Input.GetAxis(LYAxis + PlayerNumber) * (InvertVerticalAxis ? -1 : 1);
        _controller.Move(leftHorizontalAxis, leftVerticalAxis);

        var rightHorizontalAxis = Input.GetAxis(RXAxis + PlayerNumber);
        var rightVerticalAxis = Input.GetAxis(RYAxis + PlayerNumber) * (InvertVerticalAxis ? -1 : 1);
        _controller.TryMoveBlock(rightHorizontalAxis, rightVerticalAxis);

        if (Input.GetAxis(_joystickPushKey) > 0 || Input.GetKeyDown(PushKeyCode))
        {
            _controller.TryPushBlock();
        }

        if (Input.GetAxis(_joystickPullKey) > 0 || Input.GetKeyDown(PullKeyCode))
        {
            _controller.TryPullBlock();
        }

        if (Input.GetKeyDown(_joystickJumpKey) || Input.GetKeyDown(JumpKeyCode))
        {
            _controller.Jump(); 
        }
    }



}