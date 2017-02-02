using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TraversyObjectBehavior : MonoBehaviour {

    private Rigidbody rb;
    // Use this for initialization
    void Start () {
        rb = GetComponent<Rigidbody>();
    }
    
    void FixedUpdate()
    {   
        if (transform.position.x < -3.0f) {
            transform.position = new Vector3(-3.0f, transform.position.y, transform.position.z);
        }
        else if (transform.position.x > 3.0f)
        {
            transform.position = new Vector3(3.0f, transform.position.y, transform.position.z);
        }

        rb.AddForce(new Vector3(0, -100.0f, 0), ForceMode.Impulse);

        //this is stupid and proof of why you shouldn't code at 4am
        //if (transform.position.x > 0.415f && transform.position.x < 0.475f)
        //{
        //    transform.position = new Vector3(0.475f, transform.position.y, transform.position.z);
        //}
        //else if (transform.position.x < -0.415f && transform.position.x > -0.475f)
        //{
        //    transform.position = new Vector3(-0.475f, transform.position.y, transform.position.z);
        //}
    }
}
