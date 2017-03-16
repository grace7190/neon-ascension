using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour {

    public static TutorialManager Instance;
    public int TutorialStep;

    public GameObject BlueTutorial1;
    public GameObject PurpleTutorial1;
    public GameObject BlueTutorial2;
    public GameObject PurpleTutorial2;
    public GameObject BlueTutorial3a;
    public GameObject PurpleTutorial3a;
    public GameObject BlueTutorial3b;
    public GameObject PurpleTutorial3b;

    private bool _onceToken;

    //replace with actual steps later
    private static readonly List<float> tutorialTimes = new List<float> { 2.0f, 4.0f, 6.0f, 8.0f, 10.0f, 12.0f };

    // Use this for initialization
    void Start () {
        TutorialStep = 0;

        BlueTutorial1.GetComponent<Image>().canvasRenderer.SetAlpha(0.0f);
        //PurpleTutorial1.GetComponent<Image>().canvasRenderer.SetAlpha(0.0f);
        //BlueTutorial2.GetComponent<Image>().canvasRenderer.SetAlpha(0.0f);
        //PurpleTutorial2.GetComponent<Image>().canvasRenderer.SetAlpha(0.0f);
        //BlueTutorial3a.GetComponent<Image>().canvasRenderer.SetAlpha(0.0f);
        //PurpleTutorial3a.GetComponent<Image>().canvasRenderer.SetAlpha(0.0f);
        //BlueTutorial3b.GetComponent<Image>().canvasRenderer.SetAlpha(0.0f);
        //PurpleTutorial3b.GetComponent<Image>().canvasRenderer.SetAlpha(0.0f);
        //
    }

    // Update is called once per frame
    void Update()
    {   
        if (TutorialStep < getCurrentStep(Time.time))
        {
            TutorialStep = getCurrentStep(Time.time);
            ProgressTutorial();
        }

    }

    int getCurrentStep(float time)
    {
        for (int i = 0; i < tutorialTimes.Count; i++)
        {
            if (time < tutorialTimes[i])
            {
                return i;
            }
        }
        return -1;
    }

    public void ProgressTutorial ()
    {
        switch (TutorialStep)
        {
            case 0:
                break;
            case 1:
                BlueTutorial1.GetComponent<Image>().CrossFadeAlpha(1.0f, 2.0f, false);
                break;
            case 2:
                BlueTutorial1.GetComponent<Image>().CrossFadeAlpha(0.0f, 2.0f, false);
                break;
            case 3:
                BlueTutorial2.GetComponent<Image>().CrossFadeAlpha(1.0f, 1.0f, false);
                break;
            case 4:
                BlueTutorial2.GetComponent<Image>().CrossFadeAlpha(0.0f, 2.0f, false);
                break;
            case 5:
                BlueTutorial3a.GetComponent<Image>().CrossFadeAlpha(0.0f, 2.0f, false);
                break;
            case 6:
                BlueTutorial3a.GetComponent<Image>().CrossFadeAlpha(0.0f, 2.0f, false);
                break;
            default:
                Debug.Log("uh oh bad tutorial");
                break;
        }
    }

    IEnumerator FadeImage(Image image, float target, float duration)
    {
        float totalChange = target - image.color.a;
        float changePerSecond = totalChange / duration;
        float totalTime = 0;
        while (totalTime < duration)
        {
            totalTime += Time.deltaTime;
            float increment = Time.deltaTime * changePerSecond;
            image.color = new Color(image.color.r, image.color.g, image.color.b, image.color.a + increment);

            yield return new WaitForEndOfFrame();
        }

        image.color = new Color(image.color.r, image.color.g, image.color.b, target);

        yield break;
    }

}
