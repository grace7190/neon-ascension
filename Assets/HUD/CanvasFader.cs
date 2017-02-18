using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class CanvasFader : MonoBehaviour {

	public float FadeInTime;
	public float FadeOutTime;
	public bool WillFadeIn;
	public bool WillFadeOut;

	private CanvasGroup _canvasGroup;

    
    void Start()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        
        if (WillFadeIn)
        {
            FadeIn();
        }
        else if (WillFadeOut)
        {
            FadeOut();
        }
    }
	
	public void FadeOut() {
		StartCoroutine("FadeOutCoroutine");
	}
	
	public void FadeIn() {
		StartCoroutine("FadeInCoroutine");
	}
	
	private IEnumerator FadeOutCoroutine() {
		for (var f = 1f; f >= 0; f -= Time.deltaTime / FadeOutTime) {
			_canvasGroup.alpha = f;
			yield return new WaitForEndOfFrame();
		}
	    _canvasGroup.interactable = false;
	    _canvasGroup.blocksRaycasts = false;
	    _canvasGroup.alpha = 0;
	}

    private IEnumerator FadeInCoroutine() {
        _canvasGroup.blocksRaycasts = true;
        _canvasGroup.interactable = true;
        for (var f = 0f; f <= 1; f += Time.deltaTime / FadeInTime) {
			_canvasGroup.alpha = f;
			yield return new WaitForEndOfFrame();
		}
        _canvasGroup.alpha = 1f;
	}
}
