using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GarbageCollectable : MonoBehaviour {

    void OnTriggerEnter(Collider other)
    {
        if (other.tag != Tags.GarbageCollider)
            return;

        var rigidbody = GetComponent<Rigidbody>();

        // Stop object from moving
        // Specifically blocks that are relying on blocks to stay in position
        if (rigidbody != null) 
        {
            rigidbody.isKinematic = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag != Tags.GarbageCollider)
            return;

        Destroy(gameObject);
    }
}
