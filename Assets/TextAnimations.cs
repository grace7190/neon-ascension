using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

using UnityEngine;

public class TextAnimations : MonoBehaviour {

    public float FadeAndMoveAnimationTime = 1.0f;
    public float EmphasizeAnimationTime = 1.0f;
    public int EmphasizeFontSizeIncrease = 6;

    private bool _isAnimating;
    private Text _text;

    // Use this for initialization
    void Start () {
        _isAnimating = false;
        _text = GetComponent<Text>();
    }
    
    // Update is called once per frame
    void Update () {
        
    }

    public void FadeAndMoveUp(float finalY, float delay = 0.0f, Action completion = null)
    {
        FloatUp(finalY, FadeAndMoveAnimationTime, delay, completion:() => 
        {
            if (completion != null)
                completion();
        });

        FadeOut(FadeAndMoveAnimationTime, delay);
    } 

    public void Emphasize(Action completion = null)
    {
        if (_isAnimating)
            return;

        int currentSize = _text.fontSize;
        int finalSize = currentSize + EmphasizeFontSizeIncrease; 

        _isAnimating = true;
        TweenFontSize(
            currentSize,
            finalSize,
            EmphasizeAnimationTime/2,
            completion: ()=>{

                TweenFontSize(
                    finalSize,
                    currentSize,
                    EmphasizeAnimationTime/2,
                    completion: ()=>{
                        _isAnimating = false;
                        if (completion != null)
                            completion();
                    });
            });
    }

    private void TweenFontSize(int fromSize, int toSize, float animationTime, Action completion = null)
    {
        Hashtable args = 
            iTween.Hash(
                "from", fromSize,
                "to", toSize,
                "time", animationTime,
                "delay", 0,
                "easeType", iTween.EaseType.easeInOutSine,
                "onupdateinline", (Action<object>)(updatedValue => {_text.fontSize = (int)(float)updatedValue;}),
                "oncompleteinline",(Action<object>)(
                    completeParameters =>
                    {
                        if (completion != null)
                            completion();
                    }));

        iTween.ValueTo(gameObject, args);
    }

    private void FloatUp(float finalY, float duration, float delay, Action completion = null)
    {
        float FloatDistance = Mathf.Abs(transform.position.y - finalY);

        Hashtable args = 
            iTween.Hash(
                "y", FloatDistance,
                "time", duration,
                "delay", delay,
                "easeType", iTween.EaseType.easeInOutSine,
                "oncompleteinline",(Action<object>)(
                    completeParameters =>
                    {   
                        if (completion != null)
                            completion();
                    }));
        
        iTween.MoveBy(gameObject, args);
    }

    private void FadeOut(float duration, float delay = 0, Action completion = null)
    {
        Hashtable args = 
            iTween.Hash(
                "from", 1.0f,
                "to", 0.0f,
                "time", duration,
                "delay", delay,
                "easeType", iTween.EaseType.easeInOutSine,
                "onupdateinline", (Action<object>)(updatedValue =>
                {
                    var color = _text.color;
                    color.a = (float)updatedValue;
                    _text.color = color;
                }),
                "oncompleteinline",(Action<object>)(
                    completeParameters =>
                    {
                        if (completion != null)
                            completion();
                    }));

        iTween.ValueTo(gameObject, args);
    }
}
