using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanMovement : MonoBehaviour
{
    public float speed;
    public bool magnetic;

    private Rigidbody rb;
    // Use this for initialization
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        //Jump
        if (Input.GetKeyDown(KeyCode.Return))
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
    void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow) ||
            Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.DownArrow))
        {
            float moveHorizontal = Input.GetAxis("Horizontal");
            float moveVertical = Input.GetAxis("Vertical");

            Vector3 movement = new Vector3(moveVertical, 0.0f, -moveHorizontal);

            //rb.AddForce(movement * speed);
            rb.MovePosition(transform.position + movement * speed);
            //transform.position += movement * speed;
        }
    }
}
