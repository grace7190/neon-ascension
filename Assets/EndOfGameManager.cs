using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndOfGameManager : MonoBehaviour
{
    public static EndOfGameManager Instance;

    private const float RestartTransitionTime = 0.5f;
    private const float ScoreStatOffsetFromScore = 0.0f;
    private const float ScoreStatOnScreenDuration = 1.8f;
    private const float ScoreStatFadeInDuration = 0.1f;
    private const float DelayBetweenScoreStats = 1f;

    private CanvasFader _screenFader;
    private bool _isGameOver;
    private bool _isRestarting;
    private bool _canRestart;
    private GameObject player1, player2;

    public Text BlueFinalScore;
    public Text PurpleFinalScore;

    public GameObject ScoreStatPrefab;

    void Awake ()
	{
	    Instance = this;
	}

    void Start()
    {
        _screenFader = GameObject.Find("ScreenFader").GetComponent<CanvasFader>();
        player1 = GameObject.Find("Player 1");
        player2 = GameObject.Find("Player 2");
    }

    void Update()
    {
        if (_isGameOver && _canRestart && Input.GetButton("Submit"))
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
            player1.GetComponent<PlayerJoystickInputManager>().enabled = false;
            player2.GetComponent<PlayerJoystickInputManager>().enabled = false;

            var lifeBonus = ScoreManager.Instance.LivesBonusScorePair.score;
            var winBonus = ScoreManager.Instance.WinBonusScorePair.score;

            var lifeBonusMultiplier = 0;
            var lifeScoreStatText = "";
            var winScoreStatText = string.Format("+{0} winning bonus", winBonus);

            // Show score before multipliers
            BlueFinalScore.text = ScoreManager.Instance.BlueScore.ToString();
            PurpleFinalScore.text = ScoreManager.Instance.PurpleScore.ToString();


            if (winningTeam == Team.Blue)
            {
                lifeScoreStatText = string.Format("+{0} x {1} lives left bonus", lifeBonus, TeamLivesManager.Instance.numBlueLives());
                lifeBonusMultiplier = TeamLivesManager.Instance.numBlueLives();
            }
            else
            {
                lifeScoreStatText = string.Format("+{0} x {1} lives left bonus", lifeBonus, TeamLivesManager.Instance.numPurpleLives());
                lifeBonusMultiplier = TeamLivesManager.Instance.numPurpleLives();
            }

            // Apply score multipliers
            ScoreManager.Instance.IncrementScoreForTeamAndType(winningTeam, ScoreIncrementType.WinBonus);
            ScoreManager.Instance.IncrementScoreForTeamAndType(winningTeam, ScoreIncrementType.LifeBonus, lifeBonusMultiplier);

            // Animate applied score multipliers
            EmitScoreStatForTeam(winningTeam, winScoreStatText, winBonus, 0.0f, ScoreStatOnScreenDuration);
            EmitScoreStatForTeam(
                winningTeam,
                lifeScoreStatText,
                lifeBonus * lifeBonusMultiplier,
                ScoreStatOnScreenDuration + ScoreStatFadeInDuration + DelayBetweenScoreStats,
                ScoreStatOnScreenDuration);

            transform.FindChild("WinnerText").GetComponent<Text>().text = winningTeam + " Wins";
            GetComponent<CanvasFader>().FadeIn();
            StartCoroutine(AllowRestartCoroutine(2 * (ScoreStatOnScreenDuration + ScoreStatFadeInDuration + DelayBetweenScoreStats)));
        }
    }

    public bool IsGameOver()
    {
        return _isGameOver;
    }

    public void RestartGame()
    {
        StartCoroutine(RestartGameCoroutine());
    }

    // Does not update the ScoreManager's score, just animated hte final score
    private void EmitScoreStatForTeam(Team team, string statText, int scoreToAddOnCompletion, float delayBeforeStart = 0.0f, float durationOnScreen = 0.0f)
    {
        var scoreStat = Instantiate(ScoreStatPrefab, Vector3.zero, Quaternion.identity, transform);
        SetAnchorsForRectTransformTextOnTeam(scoreStat.GetComponent<RectTransform>(), team);
        SetTextAlignmentForTextOnTeam(scoreStat.GetComponent<Text>(), team);

        scoreStat.GetComponent<RectTransform>().position = StartingPositionOfTextStatForTeam(team);

        var textComponent = scoreStat.GetComponent<Text>();
        textComponent.text = statText;
        var textColor = textComponent.color;
        textColor.a = 0.0f;
        textComponent.color = textColor;

        var textAnimation = scoreStat.GetComponent<TextAnimations>();
        textAnimation.FadeIn(ScoreStatFadeInDuration, delayBeforeStart, completion:() =>
        {
            textAnimation.FadeAndMoveUp(EndPositionOfTextStatForTeam(team).y, durationOnScreen, completion:() =>
            {
                Destroy(scoreStat);
                AnimateUpdateForFinalScoreForTeam(team, scoreToAddOnCompletion);
            });
        });
    }

    private void AnimateUpdateForFinalScoreForTeam(Team team, int scoreChange)
    {

        int currentDisplayedScore = 0;
        int finalScore = 0;
        switch (team)
        {
        case Team.Blue:
            currentDisplayedScore = int.Parse(BlueFinalScore.text);
            finalScore =  currentDisplayedScore + scoreChange;
            BlueFinalScore.GetComponent<TextAnimations>().AnimateNumberTextFromTo(currentDisplayedScore, finalScore, 0.05f);
            break;
        case Team.Purple:
            currentDisplayedScore = int.Parse(PurpleFinalScore.text);
            finalScore =  currentDisplayedScore + scoreChange;
            PurpleFinalScore.GetComponent<TextAnimations>().AnimateNumberTextFromTo(currentDisplayedScore, finalScore, 0.05f);
            break;
        }
    }

    private void SetAnchorsForRectTransformTextOnTeam(RectTransform text, Team team)
    {
        switch (team)
        {
        case Team.Blue:
            text.anchorMin = new Vector2(0, 0.5f);
            text.anchorMax = new Vector2(0, 0.5f);
            text.pivot = new Vector2(0.5f, 1.0f);
            break;
        case Team.Purple:
            text.anchorMin = new Vector2(1, 0.5f);
            text.anchorMax = new Vector2(1, 0.5f);
            text.pivot = new Vector2(0.5f, 1.0f);
            break;
        }
    }

    private void SetTextAlignmentForTextOnTeam(Text text, Team team)
    {
        switch (team)
        {
        case Team.Blue:
            text.alignment = TextAnchor.MiddleLeft;
            break;
        case Team.Purple:
            text.alignment = TextAnchor.MiddleRight;
            break;
        }

    }

    private Vector3 StartingPositionOfTextStatForTeam(Team team)
    {

        var startingPosition = Vector3.zero;

        switch (team)
        {
        case Team.Blue:
            startingPosition = BlueFinalScore.GetComponent<RectTransform>().position;
            break;
        case Team.Purple:
            startingPosition = PurpleFinalScore.GetComponent<RectTransform>().position;
            break;
        }

        startingPosition.y = startingPosition.y * 0.5f - ScoreStatOffsetFromScore;
        return startingPosition;
    }

    private Vector3 EndPositionOfTextStatForTeam(Team team)
    {
        var endPosition = Vector3.zero;

        switch (team)
        {
        case Team.Blue:
            endPosition = BlueFinalScore.GetComponent<RectTransform>().position;
            break;
        case Team.Purple:
            endPosition = PurpleFinalScore.GetComponent<RectTransform>().position;
            break;
        }

        endPosition.y = endPosition.y * 0.72f;
        return endPosition;
    }

    private IEnumerator RestartGameCoroutine()
    {
        _screenFader.FadeIn();
        yield return new WaitForSeconds(_screenFader.FadeInTime);
        yield return new WaitForSeconds(RestartTransitionTime);
        SceneManager.LoadScene(Scenes.Game);
    }

    private IEnumerator AllowRestartCoroutine(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        _canRestart = true;
    }
}
