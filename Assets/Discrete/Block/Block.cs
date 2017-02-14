using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : PlayerMoveable {

    // Duration of the animation that changes from unlock state to lock state
    // 2 * LockedAnimationDuration = the total duration from unlock to lock and back to unlock state
    public const float LockedAnimationDuration = 0.3f;
    public Color LockedColor = Color.red; 

    new void Start () 
    {
        base.Start();
    }

    void Update () {
    }

    public override void SetLockedForDuration(float duration) 
    {
        if (!isLocked) 
            StartCoroutine(PlayLockedAnimation(duration));
    }

    IEnumerator PlayLockedAnimation(float duration) {

        float yieldDuration = duration - (2 * LockedAnimationDuration);

        if (yieldDuration < 0)
        {
            Debug.Log("Error at PlayLockedAnimation");
            Debug.Log("Specified duration " + duration + " is too small to complete the animation requiring " + 2 * LockedAnimationDuration);

            yield break;
        }

        isLocked                = true;
        var material            = GetComponent<Renderer>().material;
        var baseColor           = material.GetColor("_EmissionColor"); 
        float animationDuration = 0;

        // Lerp to red
        Debug.Log("Lerping to red");
        while(animationDuration < LockedAnimationDuration) 
        {
            Color emissionColor = Color.Lerp(baseColor, LockedColor, animationDuration/LockedAnimationDuration);
            animationDuration += Time.deltaTime;
            material.SetColor("_EmissionColor", emissionColor);
            yield return new WaitForEndOfFrame();
        }

        // Set to red since while loop will have exited before it can complete the lerp
        material.SetColor("_EmissionColor", LockedColor);

        // Yield till ready to change back to original
        yield return new WaitForSeconds(yieldDuration);       

        animationDuration = 0;
        Debug.Log("Lerping to orig");
        // Lerp to original
        while(animationDuration < LockedAnimationDuration) 
        {
            Color emissionColor = Color.Lerp(LockedColor, baseColor, animationDuration/LockedAnimationDuration);
            animationDuration += Time.deltaTime;
            material.SetColor("_EmissionColor", emissionColor);
            yield return new WaitForEndOfFrame();
        }

        // Set to original color since while loop will have exited before it can complete the lerp
        material.SetColor("_EmissionColor", baseColor);

        isLocked = false;
        yield break;
    }
}
