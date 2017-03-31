using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndOfGameManager : MonoBehaviour
{
    public static EndOfGameManager Instance;

    private const float RestartTransitionTime = 0.5f;

    private CanvasFader _screenFader;
    private bool _isGameOver;
    private bool _isRestarting;

    private static readonly int livesBonus = 200;
    private static readonly int winBonus = 1000;

    void Awake ()
	{
	    Instance = this;
	}

    void Start()
    {
        _screenFader = GameObject.Find("ScreenFader").GetComponent<CanvasFader>();
    }

    void Update()
    {
        if (_isGameOver && Input.GetButton("Submit"))
        {
            if (!_isRestarting)
            {
                _isRestarting = true;
                RestartGame();
            }
        }

    }

    public void ShowVictoryScreen(Team winningTeam)
    {
        if (!_isGameOver)
        {
            _isGameOver = true;
            if (winningTeam == Team.Blue)
            {
                ScoreManager.Instance.incrementBlue(winBonus);
                ScoreManager.Instance.incrementBlue(TeamLivesManager.Instance.numBlueLives()*livesBonus);
            } else
            {
                ScoreManager.Instance.incrementPurple(winBonus);
                ScoreManager.Instance.incrementPurple(TeamLivesManager.Instance.numPurpleLives()* livesBonus);
            }
            transform.FindChild("WinnerText").GetComponent<Text>().text = winningTeam + " Wins";
            GetComponent<CanvasFader>().FadeIn();
        }
    }

    public void RestartGame()
    {
        StartCoroutine(RestartGameCoroutine());
    }

    private IEnumerator RestartGameCoroutine()
    {
        _screenFader.FadeIn();
        yield return new WaitForSeconds(_screenFader.FadeInTime);
        yield return new WaitForSeconds(RestartTransitionTime);
        SceneManager.LoadScene(Scenes.Game);
    }
}
