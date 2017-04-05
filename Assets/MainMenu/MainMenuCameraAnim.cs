using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuCameraAnim : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        
        StartCoroutine(CameraGlitchCoroutine(5.0f));

    }

    
    private IEnumerator CameraGlitchCoroutine(float time)
    {
        var maxIntensity = 10.0f;
        var elapsedTime = 0.0f;
        while (elapsedTime < time)
        {
            gameObject.GetComponent<Kino.AnalogGlitch>().scanLineJitter = maxIntensity * (elapsedTime / time);
            elapsedTime += Time.deltaTime;
            yield return new WaitForSeconds(Time.deltaTime);
        }
        elapsedTime = 0.0f;
        while (elapsedTime < time)
        {
            gameObject.GetComponent<Kino.AnalogGlitch>().scanLineJitter = maxIntensity * (1 - (elapsedTime / time));
            elapsedTime += Time.deltaTime;
            yield return new WaitForSeconds(Time.deltaTime);
        }
        gameObject.GetComponent<Kino.AnalogGlitch>().scanLineJitter = 0.0f;
        StartCoroutine(CameraGlitchCoroutine(Random.Range(1,10)));

    }
}
