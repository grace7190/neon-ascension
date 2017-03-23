using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour {

    public static TutorialManager Instance;
    public int TutorialStep;

    public GameObject BlueTutorial1;
    public GameObject PurpleTutorial1;
    public GameObject Tutorial4;

    private bool _onceToken;

    //replace with actual steps later
    private static readonly List<float> tutorialTimes = new List<float> {
        3.0f, 11.0f, 12.0f, 15.0f, 17.0f, 18.0f };

    // Use this for initialization
    void Start () {
        TutorialStep = 0;

        BlueTutorial1.GetComponent<Image>().canvasRenderer.SetAlpha(0.0f);
        PurpleTutorial1.GetComponent<Image>().canvasRenderer.SetAlpha(0.0f);
        Tutorial4.GetComponent<Image>().canvasRenderer.SetAlpha(0.0f);
    }

    // Update is called once per frame
    void Update()
    {   
        if (TutorialStep < getCurrentStep(Time.timeSinceLevelLoad))
        {
            TutorialStep = getCurrentStep(Time.timeSinceLevelLoad);
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
                BlueTutorial1.GetComponent<Image>().CrossFadeAlpha(1.0f, 0.5f, false);
                PurpleTutorial1.GetComponent<Image>().CrossFadeAlpha(1.0f, 0.5f, false);
                break;
            case 2:
                BlueTutorial1.GetComponent<Image>().CrossFadeAlpha(0.0f, 2.0f, false);
                PurpleTutorial1.GetComponent<Image>().CrossFadeAlpha(0.0f, 2.0f, false);
                BlockColumnManager.Instance.CameraScrollEnabled = true;
                Debug.Log("scrolling enabled");
                break;
            case 3:
                Tutorial4.GetComponent<Image>().CrossFadeAlpha(1.0f, 0.5f, false);
                break;
            case 4:
                Tutorial4.GetComponent<Image>().CrossFadeAlpha(0.0f, 2.0f, false);
                Debug.Log("falling blocks enabled");
                BlockColumnManager.Instance.BlockRainEnabled = true;
                Debug.Log("death enabled");
                TeamLivesManager.Instance.DeathEnabled = true;
                break;
            case 5:
                Destroy(BlueTutorial1);
                Destroy(PurpleTutorial1);
                Destroy(Tutorial4);
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
