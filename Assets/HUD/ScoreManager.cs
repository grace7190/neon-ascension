using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour {

    public static ScoreManager Instance; 

    public ScoreAndDescriptionPair TimeScorePair = new ScoreAndDescriptionPair(1, "");
    public ScoreAndDescriptionPair PushWallScorePair = new ScoreAndDescriptionPair(10, "sabotage");
    public ScoreAndDescriptionPair KillPlayerByBombScorePair = new ScoreAndDescriptionPair(500, "death by kaboom");
    public ScoreAndDescriptionPair KillPlayerByCrushScorePair = new ScoreAndDescriptionPair(1000, "crushed to death");
    public ScoreAndDescriptionPair KillPlayerByPushScorePair = new ScoreAndDescriptionPair(1000, "kill bonus");
    public ScoreAndDescriptionPair NearMissScorePair = new ScoreAndDescriptionPair(100, "near miss");
    public ScoreAndDescriptionPair WinBonusScorePair = new ScoreAndDescriptionPair(5000, "");
    public ScoreAndDescriptionPair LivesBonusScorePair = new ScoreAndDescriptionPair(500, "");

    public ScoreEmitter BlueScoreEmitter;
    public ScoreEmitter PurpleScoreEmitter;

    public Text BlueText;
    public Text PurpleText;

    public int BlueScore;
    public int PurpleScore;

    public struct ScoreAndDescriptionPair
    {
        public int score;
        public string description;

        public ScoreAndDescriptionPair(int score, string description)
        {
            this.score = score;
            this.description = description;
        }
    }

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        Reset();
        StartCoroutine(TimeScoreIncrementCoroutine());
    }

    void Reset()
    {
        BlueScore = 0;
        PurpleScore = 0;
        BlueText.text = BlueScore.ToString();
        PurpleText.text = PurpleScore.ToString();
    }

    public ScoreAndDescriptionPair ScoreAndDescriptionForType(ScoreIncrementType type)
    {
        switch (type)
        {
        case ScoreIncrementType.PushWall:
            return PushWallScorePair;
        case ScoreIncrementType.KillPlayerByBomb:
            return KillPlayerByBombScorePair;
        case ScoreIncrementType.KillPlayerByCrush:
            return KillPlayerByCrushScorePair;
        case ScoreIncrementType.KillPlayerByPush:
            return KillPlayerByPushScorePair;
        case ScoreIncrementType.NearMiss:
            return NearMissScorePair;
        case ScoreIncrementType.WinBonus:
            return WinBonusScorePair;
        case ScoreIncrementType.LifeBonus:
            return LivesBonusScorePair;
        case ScoreIncrementType.TimeBonus:
            return TimeScorePair;
        default:
            return new ScoreAndDescriptionPair(0,"No description provided for this type");
        } 
    }

    public void SetScoreEmitterForTeam(ScoreEmitter scoreEmitter, Team team)
    {
        switch(team)
        {
        case Team.Blue:
            BlueScoreEmitter = scoreEmitter;
            break;
        case Team.Purple:
            PurpleScoreEmitter = scoreEmitter;
            break;
        }
    }

    public void IncrementScoreForTeamAndType(Team team, ScoreIncrementType type, int multiplier = 1)
    {
        ScoreAndDescriptionPair scorePair = ScoreAndDescriptionForType(type);
        ScoreAndDescriptionPair newScorePair = new ScoreAndDescriptionPair(scorePair.score * multiplier, scorePair.description);

        IncrementScoreForTeam(newScorePair.score, team);
        if (ShouldEmitScoreForType(type))
        {
            EmitScoreAndDescriptionForTeam(newScorePair, team);
            EmphasizeScoreTextForTeam(team);
        }
    }

    private void EmitScoreAndDescriptionForTeam(ScoreAndDescriptionPair pair, Team team)
    {
        switch(team)
        {
        case Team.Blue:
            BlueScoreEmitter.EmitScoreWithDescription(pair.score, pair.description);
            break;
        case Team.Purple:
            PurpleScoreEmitter.EmitScoreWithDescription(pair.score, pair.description);
            break;
        }
    }

    private void EmphasizeScoreTextForTeam(Team team)
    {
        switch(team)
        {
        case Team.Blue:
            BlueText.GetComponent<TextAnimations>().Emphasize();
            break;
        case Team.Purple:
            PurpleText.GetComponent<TextAnimations>().Emphasize();
            break;
        }

    }

    private void IncrementScoreForTeam(int score, Team team)
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

	private void IncrementBlue(int score)
    {
        BlueScore += score;
        BlueText.text = BlueScore.ToString();
    }

    private void IncrementPurple(int score)
    {
        PurpleScore += score;
        PurpleText.text = PurpleScore.ToString();
    }

    private bool ShouldEmitScoreForType(ScoreIncrementType type)
    {
        switch (type)
        {
        case ScoreIncrementType.PushWall:
        case ScoreIncrementType.KillPlayerByBomb:
        case ScoreIncrementType.KillPlayerByCrush:
        case ScoreIncrementType.KillPlayerByPush:
        case ScoreIncrementType.NearMiss:
            return true;
        case ScoreIncrementType.WinBonus:
        case ScoreIncrementType.LifeBonus:
        case ScoreIncrementType.TimeBonus:
            return false;
        default:
            return false;
        } 
    }

    private IEnumerator TimeScoreIncrementCoroutine()
    {
        while(true)
        {
            if (!GameController.Instance.GetIsPaused())
            {
                IncrementBlue(TimeScorePair.score);
                IncrementPurple(TimeScorePair.score);
            }
            yield return new WaitForSeconds(1);
        }
    }
}
