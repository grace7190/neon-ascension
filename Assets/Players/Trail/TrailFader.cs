using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailFader : MonoBehaviour {

    private Rigidbody _parentRigidbody;

    // Use this for initialization
    void Start () {
        _parentRigidbody = GetComponentInParent<Rigidbody>();
    }

    // Update is called once per frame
    void Update () {
        if (_parentRigidbody.velocity.x == 0 && GetComponentInChildren<TrailRenderer>().time > 0) {
            GetComponentInChildren<TrailRenderer>().time = GetComponentInChildren<TrailRenderer>().time - 0.1f; 
        }
        else {
            GetComponentInChildren<TrailRenderer>().time = 0.6f; 
        }
    }
}
