using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorMovePropagator : MonoBehaviour {

    private Animator _anim;

    // Use this for initialization
    void Start () {
        _anim = GetComponent<Animator>();
    }


    void OnAnimatorMove() {
        // Removed for now until, animations are perfected
        //transform.parent.rotation = _anim.rootRotation;
        //transform.parent.position += _anim.deltaPosition;
    }
}
