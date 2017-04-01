using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreTextGroup : MonoBehaviour {

    public float AnimationTimeIn = 0.2f;
    public float AnimationTimeOut = 1.2f;
    public int FloatDistance = 4;


    private TextMesh _scoreText;
    private TextMesh _scoreDescription;

    void Awake()
    {
        _scoreText = transform.GetChild(0).GetComponent<TextMesh>(); // More efficient than by name
        _scoreDescription = transform.GetChild(1).GetComponent<TextMesh>();
    }

    void Start()
    {
        transform.localScale = Vector3.zero;
    }

    public void SetCameraLayer(int cameraLayer)
    {
        _scoreText.gameObject.layer = cameraLayer;
        _scoreDescription.gameObject.layer = cameraLayer;
    }

    public void SetScoreAndDescription(int score, string description)
    {
        _scoreText.text = "+" + score.ToString();
        _scoreDescription.text = description;
    }

    public void StartAnimation()
    {
        ScaleIn(completion: () =>
        {
            FloatUp(completion:() => {
                Destroy(gameObject);
            });
            FadeOut();
        });
    }

    private void ScaleIn(Action completion = null)
    {
        Hashtable args = 
            iTween.Hash(
                "scale", Vector3.one,
                "time", AnimationTimeIn,
                "easeType", iTween.EaseType.easeInOutSine,
                "oncompleteinline",(Action<object>)(
                    completeParameters =>
                    {   
                        if (completion != null)
                            completion();
                    }));
        
        iTween.ScaleTo(gameObject, args);
    }

    private void FloatUp(Action completion = null)
    {
        Hashtable args = 
            iTween.Hash(
                "y", 1 * FloatDistance,
                "time", AnimationTimeOut,
                "easeType", iTween.EaseType.easeInOutSine,
                "oncompleteinline",(Action<object>)(
                    completeParameters =>
                    {   
                        if (completion != null)
                            completion();
                    }));
        
        iTween.MoveBy(gameObject, args);
    }

    private void FadeOut()
    {
        iTween.FadeTo(gameObject, 0, AnimationTimeOut);
    }
}
