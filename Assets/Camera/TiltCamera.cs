using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TiltCamera : MonoBehaviour {

	Camera cam;
	public GameObject player;

	public float tiltMaxAngle;
	private float degreePerPixel;
	private float preScreenY;
	private float curScreenY;

	// Use this for initialization
	void Start () {
		cam = GetComponent<Camera>();
		preScreenY = cam.WorldToScreenPoint (player.transform.position).y;
		degreePerPixel = tiltMaxAngle / (Screen.height - preScreenY);
	}

	// Update is called once per frame
	void Update () {
		curScreenY = cam.WorldToScreenPoint (player.transform.position).y;
		float rotateDegree = degreePerPixel * (preScreenY - curScreenY);
		transform.Rotate (new Vector3 (rotateDegree, 0, 0));
		preScreenY = curScreenY;
	}
}
