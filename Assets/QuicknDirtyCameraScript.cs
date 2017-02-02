using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuicknDirtyCameraScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKey(KeyCode.Alpha1))
        {
            transform.Translate(0, 0.05f, 0);
        }

        if (Input.GetKey(KeyCode.Alpha2))
        {
            transform.Translate(0, -0.05f, 0);
        }
	}
}
