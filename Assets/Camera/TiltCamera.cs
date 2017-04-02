using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TiltCamera : MonoBehaviour {

	Camera cam;
	public GameObject player;
	public float tiltMaxAngle;
	public float speed;

	private float degreePerPixel;
	private float initScreenY;
	//private float preScreenY;
	private float curScreenY;
	private float initX;

	// Use this for initialization
	void Start () {
		cam = GetComponent<Camera>();
		initX = transform.rotation.eulerAngles.x;
		initScreenY = cam.WorldToScreenPoint (player.transform.position).y;
		degreePerPixel = tiltMaxAngle / (Screen.height - initScreenY);
	}

	// Update is called once per frame
	void Update () {
		curScreenY = cam.WorldToScreenPoint (player.transform.position).y;
		float step = speed * Time.deltaTime;
		float curX = initX - degreePerPixel * (curScreenY - initScreenY);
		transform.rotation = Quaternion.RotateTowards (
			transform.rotation, 
			Quaternion.Euler (curX, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z), 
			step);
		//transform.Rotate (new Vector3 (rotateDegree, 0, 0));
		//preScreenY = curScreenY;
	}
}
