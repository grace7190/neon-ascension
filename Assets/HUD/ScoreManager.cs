using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour {

    public static ScoreManager Instance; 

    public static int TimeScoreIncrement = 1;
    public static int PushWallScoreIncrement = 10;
    public static int KillPlayerByBombScoreIncrement = 500;
    public static int KillPlayerByCrushScoreIncrement = 1000;
    public static int KillPlayerByPushScoreIncrement = 1000;
    public static int NearMissScoreIncrement = 100;
    public static int WinBonusScoreIncrement = 5000;
    public static int LifeBonusScoreIncrement = 500;

    public Text BlueText;
    public Text PurpleText;

    public int BlueScore;
    public int PurpleScore;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        Reset();
        StartCoroutine(TimeScoreIncrementCoroutine());
    }

    public void IncrementScoreForTeam(int score, Team team)
    {
        switch(team)
        {
        case Team.Blue:
            IncrementBlue(score);
            break;
        case Team.Purple:
            IncrementPurple(score);
            break;
        }
    }

	public void IncrementBlue(int score)
    {
        BlueScore += score;
        BlueText.text = BlueScore.ToString();
    }

    public void IncrementPurple(int score)
    {
        PurpleScore += score;
        PurpleText.text = PurpleScore.ToString();
    }

    void Reset()
    {
        BlueScore = 0;
        PurpleScore = 0;
        BlueText.text = BlueScore.ToString();
        PurpleText.text = PurpleScore.ToString();
    }

    private IEnumerator TimeScoreIncrementCoroutine()
    {
        while(true)
        {
            if (!GameController.Instance.GetIsPaused())
            {
                IncrementBlue(TimeScoreIncrement);
                IncrementPurple(TimeScoreIncrement);
            }
            yield return new WaitForSeconds(1);
        }
    }
}
