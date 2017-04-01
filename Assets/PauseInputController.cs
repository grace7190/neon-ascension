using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PauseInputController : MonoBehaviour {

	private const int ResumeIndex = 0;

    private Button[] buttons;
    private int currentIndex;
    private bool canNavigate;
    private float delayAmount;
	private float verticalVal;
    private EventSystem eventSystem;

	// Use this for initialization
	void Start () {
        buttons = new Button[] { GameObject.Find("PauseMenuResumeButton").GetComponent<Button>(),
            GameObject.Find ("PauseMenuRestartButton").GetComponent<Button>(),
            GameObject.Find ("PauseMenuExitButton").GetComponent<Button>() };
        delayAmount = 0.2f;

        eventSystem = EventSystem.current;
    }

    void OnEnable()
    {
        canNavigate = true;
        currentIndex = 0;
        // Annoying fix for Unity bug not selecting buttons well
        StartCoroutine(SelectFirstButton());
    }
	
    void Update()
    {
        if (Input.GetButton("Submit"))
        {
            buttons[currentIndex].onClick.Invoke();
        }
        if (Input.GetButton("Cancel"))
        {
            buttons[ResumeIndex].onClick.Invoke();
        }
        if (canNavigate) {
            verticalVal = Mathf.Round(Input.GetAxis("L_YAxis_1") + Input.GetAxis("L_YAxis_2"));
            if (verticalVal != 0)
            {
                currentIndex = (currentIndex + buttons.Length + (int)verticalVal) % buttons.Length;
                buttons[currentIndex].Select();
                StartCoroutine(MenuDelayCoroutine());
            }
        }
    }

    private IEnumerator SelectFirstButton()
    {
        // Waits a frame before selecting first button
        yield return null;
        eventSystem.SetSelectedGameObject(null);
        eventSystem.SetSelectedGameObject(buttons[ResumeIndex].gameObject);
    }

    private IEnumerator MenuDelayCoroutine()
    {
        canNavigate = false;
        yield return new WaitForSecondsRealtime(delayAmount);
        canNavigate = true;
    }

}
