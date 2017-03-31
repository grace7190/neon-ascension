using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialIconManager : MonoBehaviour {

    public static IconType[] TutorialIconOrder = 
        new IconType[5] 
            {
                IconType.MoveTutorial, 
                IconType.JumpTutorial,
                IconType.PullWallTutorial,
                IconType.PushWallTutorial,
                IconType.PullSideTutorial
            };

    public bool TutorialDidFinish = false;
    public int CurrentTutorialIndex = 0;

    public void DidMove()
    {
        ContinueTutorialGivenIconType(IconType.MoveTutorial);
    }

    public void DidJump()
    {
        ContinueTutorialGivenIconType(IconType.JumpTutorial);
    }

    public void DidPullWall()
    {
        ContinueTutorialGivenIconType(IconType.PullWallTutorial);
    }

    public void DidPushWall()
    {
        ContinueTutorialGivenIconType(IconType.PushWallTutorial);
    }

    public void DidPullSideBlock()
    {
        ContinueTutorialGivenIconType(IconType.PullSideTutorial);
    }

    public IconType GetCurrentTutorialIcon(){
        return TutorialIconOrder[CurrentTutorialIndex];
    }

    public void Reset()
    {
        CurrentTutorialIndex = 0;
        TutorialDidFinish = false;
    }

    private void ContinueTutorialGivenIconType(IconType type) {

        Debug.Log(TutorialIconOrder[CurrentTutorialIndex]);
        if (TutorialIconOrder[CurrentTutorialIndex] == type)
        {
            CurrentTutorialIndex ++;
            if (CurrentTutorialIndex >= TutorialIconOrder.Length) {
                TutorialDidFinish = true;
            }
        }
    }
}
