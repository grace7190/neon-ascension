using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {

    public GameObject robotEndLevel;
    public GameObject humanEndLevel;

	// Use this for initialization
	void Start () {
        Physics.IgnoreLayerCollision(8, 9);
    }
	
	// Update is called once per frame
	void Update () {
        if (robotEndLevel == null && humanEndLevel == null)
        {
            GUI.Label(new Rect(0, 0, 10, 10), "Level complete!");
        }
	}
}
