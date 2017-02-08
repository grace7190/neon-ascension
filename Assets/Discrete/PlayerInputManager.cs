﻿using UnityEngine;

public class PlayerInputManager : MonoBehaviour
{
    public KeyCode UpKey;
    public KeyCode DownKey;
    public KeyCode LeftKey;
    public KeyCode RightKey;

    public KeyCode PushKey;
    public KeyCode PullKey;

    private PlayerController _controller;

	void Start ()
	{
	    _controller = GetComponent<PlayerController>();
	}
	
	void Update ()
	{
	    if (Input.GetKeyDown(RightKey))
	    {
	        _controller.Move(Vector3.right);
	    }
        else if (Input.GetKeyDown(LeftKey))
	    {
	        _controller.Move(Vector3.left);
        }
        else if (Input.GetKeyDown(UpKey))
        {
            _controller.Move(Vector3.forward);
        }
        else if (Input.GetKeyDown(DownKey))
        {
            _controller.Move(Vector3.back);
        }
        

	    if (Input.GetKeyDown(PushKey))
	    {
	        _controller.TryPushBlock();
	    }
	    else if (Input.GetKeyUp(PullKey))
	    {
	        _controller.TryPullBlock();
	    }
	}
}
