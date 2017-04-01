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

    public Text BlueFinalScore;
    public Text PurpleFinalScore;

    public Text BlueStats;
    public Text PurpleStats;

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
            var lifeBonus = ScoreManager.LifeBonusScoreIncrement;
            var winBonus = ScoreManager.WinBonusScoreIncrement;
            if (winningTeam == Team.Blue)
            {
                ScoreManager.Instance.IncrementBlue(winBonus);
                ScoreManager.Instance.IncrementBlue(TeamLivesManager.Instance.numBlueLives()*lifeBonus);
                BlueStats.text = string.Format("+{0} lives left bonus\n+1000 winning bonus", TeamLivesManager.Instance.numBlueLives() * lifeBonus);
            } else
            {
                ScoreManager.Instance.IncrementPurple(winBonus);
                ScoreManager.Instance.IncrementPurple(TeamLivesManager.Instance.numPurpleLives() * lifeBonus);
                PurpleStats.text = string.Format("+{0} lives left bonus\n+1000 winning bonus", TeamLivesManager.Instance.numPurpleLives() * lifeBonus);
            }
            transform.FindChild("WinnerText").GetComponent<Text>().text = winningTeam + " Wins";
            GetComponent<CanvasFader>().FadeIn();
            BlueFinalScore.text = ScoreManager.Instance.BlueScore.ToString();
            PurpleFinalScore.text = ScoreManager.Instance.PurpleScore.ToString();

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
