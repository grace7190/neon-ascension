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
	}
	
	void Update ()
	{
	    if (Input.GetKeyDown(RightKey))
        {   
            bool wouldTurn = _controller.wouldTurn(Vector3.right);
            if (Input.GetKey(PushKey))
            {
                if (wouldTurn) //if facing left, try to pull left block [ ]->p
                {
                    _controller.TryPullBlock();
                } //if facing right, try to push block to right p->[ ]
                _controller.TryPushBlock();
            }
            else
            {   //only move if didn't turn
                bool didTurn = _controller.Turn(Vector3.right);
                if (!didTurn)
                {
                    _controller.Move(Vector3.right);
                }

            }
        }
        else if (Input.GetKeyDown(LeftKey))
        {  
            bool wouldTurn = _controller.wouldTurn(Vector3.left);
            if (Input.GetKey(PushKey))
            {
                if (wouldTurn)
                {
                    _controller.TryPullBlock();
                }
                _controller.TryPushBlock();
            }
            else
            {
                bool didTurn = _controller.Turn(Vector3.left);
                if (!didTurn)
                {
                    _controller.Move(Vector3.left);
                }

            }
        }
        else if (Input.GetKeyDown(UpKey))
        {
            if (Input.GetKey(PushKey))
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
        else if (Input.GetKeyDown(DownKey))
        {
            if (Input.GetKey(PushKey))
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
