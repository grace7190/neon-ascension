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

	void Awake ()
	{
	    Instance = this;
	}

    void Start()
    {
        _screenFader = GameObject.Find("ScreenFader").GetComponent<CanvasFader>();
    }

    public void ShowVictoryScreen(Team winningTeam)
    {
        if (!_isGameOver)
        {
            _isGameOver = true;
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
