using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

using UnityEngine;

public class TextAnimations : MonoBehaviour {

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

}
