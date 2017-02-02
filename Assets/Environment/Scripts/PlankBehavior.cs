using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlankBehavior : MonoBehaviour {
    public GameObject player;
    public bool isPickedUp;
    Transform plankTransform;
    Transform triggerTransform;

    // Use this for initialization
    void Start () {

        plankTransform = transform.Find("Plank");
        triggerTransform = transform.Find("Trigger");
        isPickedUp = false;
        player = null;
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        if (isPickedUp && player != null)
        {   //move object to player
            transform.position = player.transform.position;
            //maybe rotate it or something ugh
            triggerTransform.rotation = plankTransform.rotation;
        }
        else
        {   //move trigger to plank
            triggerTransform.position = plankTransform.position;
            triggerTransform.rotation = plankTransform.rotation;
        }

    }
}
