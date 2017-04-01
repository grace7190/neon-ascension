using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeedbackIconManager : MonoBehaviour {

    public GameObject StopPrefab;
    public GameObject JumpTutorialPrefab;
    public GameObject MoveStickTutorialPrefab;
    public GameObject PullFromSidePrefab;
    public GameObject PullFromWallPrefab;
    public GameObject PushFromWallPrefab;
    public bool IsAnimating = false;

    private FeedbackIcon _currentIcon;

    void Start()
    {
        transform.rotation = Quaternion.identity;
    }

    void LateUpdate()
    {
        // Billboard
        transform.rotation = Quaternion.identity;
    } 

    public void ShowStopIcon(bool hideAutomatically = false, float hideDelay = 0.0f)
    {
        InstantiateAndShowIcon(StopPrefab, IconType.Stop, hideAutomatically, hideDelay);
    }

    public void ShowJumpTutorialIcon(bool hideAutomatically = false, float hideDelay = 0.0f)
    {
        InstantiateAndShowIcon(JumpTutorialPrefab, IconType.JumpTutorial, hideAutomatically, hideDelay);
    }

    public void ShowMoveTutorialIcon(bool hideAutomatically = false, float hideDelay = 0.0f)
    {
        InstantiateAndShowIcon(MoveStickTutorialPrefab, IconType.MoveTutorial, hideAutomatically, hideDelay);
    }

    public void ShowPullWallTutorialIcon(bool hideAutomatically = false, float hideDelay = 0.0f)
    {
        InstantiateAndShowIcon(PullFromWallPrefab, IconType.PullWallTutorial, hideAutomatically, hideDelay);
    }

    public void ShowPushWallTutorialIcon(bool hideAutomatically = false, float hideDelay = 0.0f)
    {
        InstantiateAndShowIcon(PushFromWallPrefab, IconType.PushWallTutorial, hideAutomatically, hideDelay);
    }

    public void ShowPullSideTutorialIcon(bool hideAutomatically = false, float hideDelay = 0.0f)
    {
        InstantiateAndShowIcon(PullFromSidePrefab, IconType.PullSideTutorial, hideAutomatically, hideDelay);
    }

    // This could probably have been more intertwined with the prefabs
    // But there is no public unity editor property for dictionaries
    public void ShowIconForType(IconType type, bool hideAutomatically = false, float hideDelay = 0.0f)
    {
        switch(type)
        {
            case IconType.Stop:
                ShowStopIcon(hideAutomatically, hideDelay);
                break;
            case IconType.JumpTutorial:
                ShowJumpTutorialIcon(hideAutomatically, hideDelay);
                break;
            case IconType.MoveTutorial:
                ShowMoveTutorialIcon(hideAutomatically, hideDelay);
                break;
            case IconType.PullSideTutorial:
                ShowPullSideTutorialIcon(hideAutomatically, hideDelay);
                break;
            case IconType.PullWallTutorial:
                ShowPullWallTutorialIcon(hideAutomatically, hideDelay);
                break;
            case IconType.PushWallTutorial:
                ShowPushWallTutorialIcon(hideAutomatically, hideDelay);
                break;
        }
    } 

    public void HideCurrentIcon(float delay = 0.0f, Action completion = null)
    {
        if (_currentIcon) {
            IsAnimating = true;
            _currentIcon.ScaleOut(delay, () =>
            {
                Destroy(_currentIcon.gameObject); 
                _currentIcon = null;
                IsAnimating = false;
                if (completion != null)
                {
                    completion();
                }
            });
        }
    }

    private void InstantiateAndShowIcon(GameObject prefab, IconType iconType, bool hideAutomatically = false, float hideDelay = 0.0f)
    {
        if (IsAnimating)
        {
            return;
        }

        if (_currentIcon == null)
        {
            ShowIcon(prefab, hideAutomatically, hideDelay);
        }
        else if (_currentIcon.iconType != iconType)
        {
            HideCurrentIcon (completion:() => {
                ShowIcon(prefab, hideAutomatically, hideDelay);
            });
        }
    }

    private void ShowIcon(GameObject iconPrefab, bool hideAutomatically = false, float hideDelay = 0.0f)
    {
        IsAnimating = true;
        var iconGameObject = Instantiate(iconPrefab, transform.position, Quaternion.identity);
        iconGameObject.transform.SetParent(transform);
        _currentIcon = iconGameObject.GetComponent<FeedbackIcon>();
        _currentIcon.ScaleIn(() =>
        {
            IsAnimating = false;
            if (hideAutomatically)
            {
                HideCurrentIcon(delay: hideDelay);
            }
        });
    }
}
