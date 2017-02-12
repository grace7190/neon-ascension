using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveable : MonoBehaviour {
    /*
     * Final Destination of this player moveable object when a 
     * a player is in the process of moving it.
     * Values may be altered by other forces, so the values may
     * become inacurrate
     */
    public Vector3 finalDestination;

    void Start () {
        finalDestination = transform.position;
    }
}
