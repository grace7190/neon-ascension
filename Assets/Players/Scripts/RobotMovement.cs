using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotMovement : MonoBehaviour {
    public float speed;
    public GameObject friend;
    public bool magnetic;
    public float magneticForce;

    private Rigidbody rb;
    // Use this for initialization
    void Start () {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        //Jump
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //Raycast to check for ground contact
            Debug.Log("jump!");
            RaycastHit hit;
            Ray ray = new Ray(transform.position, -Vector3.up);
            if (Physics.Raycast(ray, out hit))
            {
                Debug.Log(hit.distance);
                if (hit.distance <= 0.5f)
                {
                    rb.AddForce(new Vector3(0, 10.0f, 0), ForceMode.Impulse);
                }

            }
        }
    }
	
	// Update is called once per frame
	void FixedUpdate () {

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D) ||
            Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S))
        {
            float moveHorizontal = Input.GetAxis("Horizontal");
            float moveVertical = Input.GetAxis("Vertical");

            Vector3 movement = new Vector3(-moveVertical, 0.0f, moveHorizontal);

            //rb.AddForce(movement * speed);
            //transform.position += movement * speed; 
            rb.MovePosition(transform.position + movement * speed);
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            magnetic = !magnetic;
        }

        if (magnetic)
        {
            Vector3 dir = friend.transform.position - transform.position;
            dir = dir.normalized;
            rb.AddForce(dir * magneticForce);
        }
    }
}