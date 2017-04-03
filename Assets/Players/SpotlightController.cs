using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpotlightController : MonoBehaviour {

    public GameObject player;

	// Use this for initialization
	void Start () {
        gameObject.transform.position = new Vector3(player.transform.position.x, player.transform.position.y, transform.position.z); 
	}
	
	// Update is called once per frame
	void Update () {
        gameObject.transform.position = new Vector3(player.transform.position.x, player.transform.position.y, transform.position.z);
    }
}
