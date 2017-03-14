using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public static GameController Instance;

    public bool IsStarted = false;
	public KeyCode pauseKey;

	private bool gamePaused;
	private GameObject pauseMenu;
	private GameObject player1;
	private GameObject player2;

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
	
	void Update () {
		if ((Input.GetKeyDown (pauseKey) || Input.GetButton("Pause")) && !gamePaused) {
			Time.timeScale = 0.0f;
			gamePaused = true;
			pauseMenu.SetActive (true);
			player1.GetComponent<PlayerJoystickInputManager>().enabled = false;
			player2.GetComponent<PlayerJoystickInputManager>().enabled = false;
		}
	}

	public void Resume() {
		pauseMenu.SetActive (false);
		Time.timeScale = 1.0f;
		gamePaused = false;
		player1.GetComponent<PlayerJoystickInputManager>().enabled = true;
		player2.GetComponent<PlayerJoystickInputManager>().enabled = true;
	}

	public void Restart() {
		Time.timeScale = 1.0f;
		SceneManager.LoadScene(Scenes.Game);
	}

	public void Exit() {
		Application.Quit ();
	}
}
