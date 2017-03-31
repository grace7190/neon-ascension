using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeedbackIcon : MonoBehaviour {

    public float AnimationTimeIn = 0.1f;
    public float AnimationTimeOut = 0.1f;
    public float AnimationTimeEmphasize = 0.2f;
    public IconType iconType;

    public Vector3 finalScale = new Vector3(0.2f, 0.2f, 0.2f);

    private string _scaleInTweenName = "scaleIn";
    private string _scaleOutTweenName = "scaleOut";
    private string _scaleEmphasizeTweenName = "scaleEmphasize";

    private string _currentTween = null;

    public void ScaleIn(Action completion = null)
    {
        StopCurrentTweenIfAnimating();
        Hashtable args = 
            iTween.Hash(
                "name", _scaleInTweenName,
                "scale", finalScale,
                "time", AnimationTimeIn,
                "easeType", iTween.EaseType.easeInOutSine,
                "oncompleteinline",(Action<object>)(
                    completeParameters =>
                    {   
                        _currentTween = null;
                        if (completion != null)
                            completion();
                    }));
        _currentTween = _scaleInTweenName;
        iTween.ScaleTo(gameObject, args);
    }

    public void EmphasizeByScale(Action completion = null)
    {
        StopCurrentTweenIfAnimating();
        Hashtable args = 
            iTween.Hash(
                "name", _scaleEmphasizeTweenName,
                "scale", finalScale * 1.1f,
                "time", AnimationTimeEmphasize/2,
                "easeType", iTween.EaseType.easeInOutSine,
                "oncompleteinline",(Action<object>)(
                    completeParameters =>
                    {   
                        Hashtable argsEmphasizeDown = 
                            iTween.Hash(
                                "name", _scaleEmphasizeTweenName,
                                "scale", finalScale,
                                "time", AnimationTimeEmphasize/2,
                                "easeType", iTween.EaseType.easeInOutSine,
                                "oncompleteinline",(Action<object>)(
                                    completeEmphasizeDownParameters =>
                                    {   
                                        _currentTween = null;
                                        if (completion != null)
                                            completion();
                                    }));
                        _currentTween = _scaleEmphasizeTweenName;
                        iTween.ScaleTo(gameObject, argsEmphasizeDown);
                    }));
        _currentTween = _scaleEmphasizeTweenName;
        iTween.ScaleTo(gameObject, args);
    }

    public void ScaleOut(float delay = 0.0f, Action completion = null)
    {
        StopCurrentTweenIfAnimating();
        Hashtable args = 
            iTween.Hash(
                "name", _scaleOutTweenName,
                "scale", Vector3.zero,
                "time", AnimationTimeOut,
                "delay", delay,
                "easeType", iTween.EaseType.easeInOutSine,
                "oncompleteinline",(Action<object>)(
                    completeParameters =>
                    {
                        if (completion != null)
                            completion();
                    }));
        _currentTween = _scaleOutTweenName;
        iTween.ScaleTo(gameObject, args);
    }

    private void StopCurrentTweenIfAnimating()
    {
        if (_currentTween != null)
        {
            iTween.StopByName(_currentTween);
            _currentTween = null;
        }
    }

}
