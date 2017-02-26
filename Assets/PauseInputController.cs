using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PauseInputController : MonoBehaviour {

	private Button resumeButton;
	private Button restartButton;
	private Button exitButton;
	private Button currentButton;

	private float verticalVal;
	private int repeat;
	private int current_iteration;
	private bool clicked;

	// Use this for initialization
	void Start () {
		resumeButton = GameObject.Find ("PauseMenuResumeButton").GetComponent<Button>();
		restartButton = GameObject.Find ("PauseMenuRestartButton").GetComponent<Button>();
		exitButton = GameObject.Find ("PauseMenuExitButton").GetComponent<Button>();
		currentButton = resumeButton;
		repeat = 10;
		current_iteration = 10;
		clicked = false;
	}
	
	// Update is called once per frame
	void Update () {
		verticalVal = Mathf.Round (Input.GetAxis ("Vertical_P1") + Input.GetAxis ("Vertical_P2"));
		clicked = Input.GetButton ("Choose");
		if (current_iteration >= repeat) {
			if (clicked) {
				currentButton.onClick.Invoke ();
			}
			else if (verticalVal <= -1) {
				if (currentButton.name == "PauseMenuExitButton") {
					currentButton = restartButton;
				} else if (currentButton.name == "PauseMenuRestartButton") {
					currentButton = resumeButton;
				}
			} else if (verticalVal >= 1) {
				if (currentButton.name == "PauseMenuResumeButton") {
					currentButton = restartButton;
				} else if (currentButton.name == "PauseMenuRestartButton") {
					currentButton = exitButton;
				}
			}
			current_iteration = 0;
			currentButton.Select ();
		}
		current_iteration++;
	}
}
