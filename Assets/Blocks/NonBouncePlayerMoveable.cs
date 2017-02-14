using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NonBouncePlayerMoveable : MonoBehaviour {

    void Start () {
        // Increase accuracy, the higher the number the less bounciness occurs
        GetComponent<Rigidbody>().solverVelocityIterations = 6;
    }

    void FixedUpdate() {
        Rigidbody rigidbody = GetComponent<Rigidbody>();

        // Do not allow this object to move upwards
        if (rigidbody.velocity.y > 0)
        {
            var newVelocity = rigidbody.velocity;
            newVelocity.y = 0; 
            rigidbody.velocity = newVelocity;
        }
    }

    void OnTriggerEnter(Collider other) 
    {
        Rigidbody rigidbody = GetComponent<Rigidbody>();

        /*
         * ColliderBuffer was triggered
         * Place this object directly ontop of other object to remove impact force
         */
        if (other.tag == "Block" &&
            IsStrictlyFalling(rigidbody) && 
            IsSameColumn(GetComponent<PlayerMoveable>(), other.GetComponent<PlayerMoveable>())) 
        {
            float finalPosY = other.transform.position.y + GetComponent<Collider>().bounds.size.y/2 + other.bounds.size.y/2;
            var finalPos = transform.position;
            finalPos.y = finalPosY;
            rigidbody.velocity = Vector3.zero;
            transform.position = finalPos;
        }
    }

    // Returns true if rigidbody is falling and not moving in any other direction
    private bool IsStrictlyFalling(Rigidbody rigidbody) 
    {
        return rigidbody.velocity.y < 0 && 
               rigidbody.velocity.x == 0 &&
               rigidbody.velocity.z == 0;
    }

    // Is this moveable in or will be in the same column?
    private bool IsSameColumn(PlayerMoveable a, PlayerMoveable b) 
    {
        // Look at where the block will be in terms of X and Z
        return a.finalDestination.x == b.finalDestination.x &&
               a.finalDestination.z == b.finalDestination.z;
    }
}
