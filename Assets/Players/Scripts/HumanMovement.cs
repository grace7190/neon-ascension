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
            transform.position += movement * speed;
        }
        if (Input.GetKeyDown(KeyCode.Return) && rb.velocity.magnitude < 0.05)
        {
            rb.AddForce(new Vector3(0, 700.0f, 0), ForceMode.Impulse);
        }
    }
}
