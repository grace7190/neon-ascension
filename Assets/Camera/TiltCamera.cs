using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TiltCamera : MonoBehaviour {

	public GameObject player;
	public GameObject blockColumnManager;
	public float maxAngleChange;			// Max degree of angle to change from initial angle
	public float maxMovement;				// Max offset to move the camera for tilting
	public int maxFrameUpdate;				// Number of frames used to update tilting to target.
	public float minScreenBaseY;			// Min Y of base's location on screen
	public float maxScreenBaseY;			// Max Y of base's location on screen

	private Camera cam;
	private BoxCollider supportBoxCollider;
	private float maxDiff = 15.0f;			// maxDiff is set to the wall height
	private float anglePerDiff;	
	private float movePerDiff;
	private float initAngle;
	private float initDiff;
	private float preOffset;				// Previous offset
	private int numFrameUpdate;				// Count number of frames being used for update
	private float anglePerFrame;			// How many degrees to tilt per frame
	private float movePerFrame;				// The distance to move per frame for tilting

	// Use this for initialization
	void Start () {
		cam = GetComponent<Camera> ();
		supportBoxCollider = blockColumnManager.GetComponent<BoxCollider>();
		initDiff = Mathf.Floor(player.transform.position.y - supportBoxCollider.center.y);
		anglePerDiff = maxAngleChange / maxDiff;
		movePerDiff = maxMovement / maxDiff;
		initAngle = transform.rotation.eulerAngles.x;
		preOffset = 0;
		anglePerFrame = 0;
		movePerFrame = 0;
		numFrameUpdate = maxFrameUpdate;
	}

	// Update is called once per frame
	void Update () {
		// Tilt camera only when player has at least 1 life.
		if (player != null) {
			// Recalculate target tilting angle and camera distance only when previous update is done.
			if (numFrameUpdate >= maxFrameUpdate) {
				float curDiff = Mathf.Floor (player.transform.position.y - supportBoxCollider.center.y);
				float actDiff = curDiff - initDiff;
				float curAngle = initAngle + anglePerDiff * actDiff;
				float curOffset = movePerDiff * actDiff;
				float offsetDiff = curOffset - preOffset;
				anglePerFrame = Mathf.DeltaAngle(transform.rotation.eulerAngles.x, curAngle) / maxFrameUpdate;
				movePerFrame = offsetDiff / maxFrameUpdate;
				preOffset = curOffset;
				numFrameUpdate = 0;
			} else {
				numFrameUpdate += 1;
			}
			transform.rotation = Quaternion.Euler (
				transform.rotation.eulerAngles.x + anglePerFrame,
				transform.rotation.eulerAngles.y,
				transform.rotation.eulerAngles.z
			);
			// Don't update distance once base is outside of designed range on screen.
			if (cam.WorldToScreenPoint(supportBoxCollider.center).y > minScreenBaseY 
				&& cam.WorldToScreenPoint(supportBoxCollider.center).y < maxScreenBaseY){
				transform.position = new Vector3 (
					transform.position.x, 
					transform.position.y + movePerFrame, 
					transform.position.z
				);
			}
		}
	}
}
