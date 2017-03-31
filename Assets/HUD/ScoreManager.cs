using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour {

    public static ScoreManager Instance; 

    public Text blueText;
    public Text purpleText;

    public int blueScore;
    public int purpleScore;

    void Awake()
    {
        Instance = this;
    }


    void Start ()
    {
        Reset();
    }
	
	public void incrementBlue(int score)
    {
        blueScore += score;
        blueText.text = blueScore.ToString();
    }

    public void incrementPurple(int score)
    {
        purpleScore += score;
        purpleText.text = purpleScore.ToString();
    }

    void Reset()
    {
        blueScore = 0;
        purpleScore = 0;
        blueText.text = blueScore.ToString();
        purpleText.text = purpleScore.ToString();
    }

}
