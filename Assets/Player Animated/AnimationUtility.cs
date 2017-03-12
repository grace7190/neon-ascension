using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationUtility {

    static Dictionary<Animator, Dictionary<string, AnimationClip>> AnimatorHash;

    static AnimationUtility()
    {
        AnimatorHash = new Dictionary<Animator, Dictionary<string, AnimationClip>>();
    }

    public static AnimationClip AnimationClipWithName(Animator animator, string name)
    {

        if (AnimatorHash.ContainsKey(animator) && AnimatorHash[animator].ContainsKey(name))
        {
            return AnimatorHash[animator][name];
        }

        AnimationClip returnClip = null;

        foreach (AnimationClip clip in animator.runtimeAnimatorController.animationClips)
        {
            if (!AnimatorHash.ContainsKey(animator)) 
            {
                var clipHash = new Dictionary<string, AnimationClip>();
                AnimatorHash.Add(animator, clipHash);
            }

            AnimatorHash[animator].Add(clip.name, clip);

            if (clip.name == name) {
                returnClip = clip;
            }
        }

        return returnClip;
    }
}
