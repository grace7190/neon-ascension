using UnityEngine;

public class PlayerInputManager : MonoBehaviour
{
    public bool Invert;

    public KeyCode UpKey;
    public KeyCode DownKey;
    public KeyCode LeftKey;
    public KeyCode RightKey;

    public KeyCode PushKey;
    public KeyCode JumpKey;

    private PlayerController _controller;
	private float horizontalVal;
	private float verticalVal;
	private int repeat;
	private int current_iteration;
	private bool grabOn;

	void Start ()
	{
	    _controller = GetComponent<PlayerController>();
	    if (Invert)
	    {
	        //var temp = UpKey;
	        //UpKey = DownKey;
	        //DownKey = temp;
	        var temp = LeftKey;
	        LeftKey = RightKey;
	        RightKey = temp;
	    }
		repeat = 15;
		current_iteration = 15;
	}
	
	void Update ()
	{
		if (gameObject.name == "Player 1") {
			horizontalVal = Mathf.Round (Input.GetAxis ("Horizontal_P1"));
			verticalVal = Mathf.Round (Input.GetAxis ("Vertical_P1"));
			grabOn = Input.GetButton ("Grab_P1");
		} else {
			horizontalVal = Mathf.Round (Input.GetAxis ("Horizontal_P2"));
			verticalVal = Mathf.Round (Input.GetAxis ("Vertical_P2"));
			grabOn = Input.GetButton ("Grab_P2");
		}
		current_iteration += 1;
        if (Input.GetKeyDown(RightKey) || (current_iteration >= repeat && horizontalVal == 1))
        {

            if (Input.GetKey(PushKey) || grabOn)
            {
                bool IsFacing = _controller.IsFacing(Vector3.right);
                if (IsFacing) //if facing left, try to pull left block [ ]->p
                {
                    _controller.TryPullBlock();
                } //if facing right, try to push block to right p->[ ]
                _controller.TryPushBlock();
            }
            else
            {   //only move if didn't turn
                bool didTurn = _controller.Turn(Vector3.right);
                Debug.Log(didTurn);
                if (!didTurn)
                {
                    _controller.Move(Vector3.right);
                }

            }
			current_iteration = 0;
        }
		else if (Input.GetKeyDown(LeftKey) || (current_iteration >= repeat && horizontalVal == -1))
        {  

            if (Input.GetKey(PushKey) || grabOn)
            {
                bool IsFacing = _controller.IsFacing(Vector3.left);
                if (IsFacing)
                {
                    _controller.TryPullBlock();
                }
                _controller.TryPushBlock();
            }
            else
            {
                bool didTurn = _controller.Turn(Vector3.left);
                Debug.Log(didTurn);
                if (!didTurn)
                {
                    _controller.Move(Vector3.left);
                }

            }
			current_iteration = 0;
        }
		else if (Input.GetKeyDown(UpKey) || (current_iteration >= repeat && verticalVal == -1))
        {
            if (Input.GetKey(PushKey) || grabOn)
            {
                if (Invert)
                {
                    _controller.Turn(Vector3.back);
                    _controller.TryPushBlock();
                }
                else
                {
	                _controller.Turn(Vector3.forward);
	                _controller.TryPushBlock();
                }
            }
            
        }
		else if (Input.GetKeyDown(DownKey) || (current_iteration >= repeat && verticalVal == 1))
        {
            if (Input.GetKey(PushKey) || grabOn)
            {
                if (Invert)
                {
                    _controller.Turn(Vector3.back);
                    _controller.TryPullBlock();
                } else
                {
                    _controller.Turn(Vector3.forward);
                    _controller.TryPullBlock();
                }

            }
        }
        else if (Input.GetKeyDown(JumpKey))
        {
           // _controller.Jump(); 
        }
	}
}