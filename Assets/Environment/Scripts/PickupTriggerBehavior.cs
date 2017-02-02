using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupTriggerBehavior : MonoBehaviour {
    
    public bool isPickedUp;

    private PlankBehavior parent;

    void Start()
    {
        //initialize parent
        parent = transform.parent.GetComponent<PlankBehavior>();
    }

    // Update is called once per frame

    void FixedUpdate()
    {   //drop object
        if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.Slash))
        {
            isPickedUp = false;
            parent.player = null;
            
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player")
        {   //attach object
            if (Input.GetKeyDown(KeyCode.Q) || Input.GetKeyDown(KeyCode.Period))
            {
                Debug.Log("Grabby Grabby Board");
                parent.player = other.gameObject;
                parent.isPickedUp = true;
            }
        }

    }
}
