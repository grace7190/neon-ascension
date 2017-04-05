using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameController : MonoBehaviour
{
    public static GameController Instance;

    public float EnableDelay = 10.0f;
    public bool IsStarted = false;
	public KeyCode pauseKey;

	private bool gamePaused;
	private GameObject pauseMenu;
	private GameObject player1;
	private GameObject player2;

    public AudioSource SFXWalk; 

    void Awake()
    {
        Instance = this;
    }

	void Start () {
		gamePaused = false;
		pauseMenu = GameObject.Find ("PauseMenu");
		player1 = GameObject.Find ("Player 1");
		player2 = GameObject.Find ("Player 2");
		pauseMenu.SetActive (false);
	}


    public void WalkingSound()
    {
        SFXWalk.Play();
    }
    

    public void TogglePause()
    {
        if (!gamePaused)
        {
            Time.timeScale = 0.0f;
            gamePaused = true;
            pauseMenu.SetActive(true);
            player1.GetComponent<PlayerJoystickInputManager>().enabled = false;
            player2.GetComponent<PlayerJoystickInputManager>().enabled = false;
        }
        else
        {
            pauseMenu.SetActive(false);
            Time.timeScale = 1.0f;
            gamePaused = false;
            player1.GetComponent<PlayerJoystickInputManager>().enabled = true;
            player2.GetComponent<PlayerJoystickInputManager>().enabled = true;
        }
    }

    public bool GetIsPaused()
    {
        return gamePaused;
    }

	public void Restart() {
		Time.timeScale = 1.0f;
		SceneManager.LoadScene(Scenes.Game);
	}

	public void Exit() {
		Application.Quit ();
	}

    public void EnableGame() {

        TutorialIconManager.CanPerformTutorial = true;

        StartCoroutine(EnableAfterDelay(EnableDelay));

    }

    IEnumerator EnableAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        BlockColumnManager.Instance.CameraScrollEnabled = true;
        BlockColumnManager.Instance.BlockRainEnabled = true;
        TeamLivesManager.Instance.enableHUD();
    }
}
