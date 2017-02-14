using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerMoveable : MonoBehaviour {
    /*
     * Final Destination of this player moveable object when a 
     * a player is in the process of moving it.
     * Values may be altered by other forces, so the values may
     * become inacurrate
     */
    public Vector3 finalDestination;

    // True when this object is locked, in effect
    // Player's should not be able to move this object at this time
    public bool isLocked {get; protected set;}

    protected void Start ()
    {
        finalDestination = transform.position;
        isLocked = false;
    }

    public virtual void SetLockedForDuration(float duration)
    {
        Debug.Log("Need to implement: SetLockedForDuration");
    }
}
