using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class GlowLight : MonoBehaviour
{

    public float OnDuration = 0.4f;
    public float MaximumIntensity = 6;
    public float AnimationTimeIn = 0.08f;
    public float AnimationTimeOut = 0.37f;

    private Light _light;

    void Start ()
    {
        if (_light == null)
        {
            Init();
        }
    }

    public void Init()
    {
        _light = GetComponent<Light>();
    }
   
    public void StartAnimation()
    {
        Hashtable args = 
            iTween.Hash(
                "from", 0,
                "to", MaximumIntensity,
                "time", AnimationTimeIn,
                "delay", 0,
                "easeType", iTween.EaseType.easeInOutSine,
                "onupdateinline", (Action<object>)(updatedValue => {_light.intensity = (float)updatedValue;}),
                "oncompleteinline",(Action<object>)(
                    completeParameters =>
                    {
                        TurnLightOffAfterDelay(OnDuration);
                    }));

        iTween.ValueTo(gameObject, args);
    }

    public void SetLightColor(Color color)
    {
        _light.color = color;
    }

    private void TurnLightOffAfterDelay(float delay)
    {
        Hashtable args = 
            iTween.Hash(
                "from", MaximumIntensity,
                "to", 0,
                "time", AnimationTimeOut,
                "delay", delay,
                "easeType", iTween.EaseType.easeInOutSine,
                "onupdateinline", (Action<object>)(updatedValue => {_light.intensity = (float)updatedValue;}),
                "oncompleteinline",(Action<object>)(
                    completeParameters =>
                    {
                        Destroy(gameObject);
                    }));
        iTween.ValueTo(gameObject, args);
    }
}
