using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyUp(KeyCode.Return) || Input.GetKeyDown("joystick 1 button 7") || Input.GetKeyDown("joystick 2 button 7"))
        {
            //Application.LoadLevel("Game");
            Debug.Log("wtf do something");
            SceneManager.LoadSceneAsync("Game");
        }
	}
}
