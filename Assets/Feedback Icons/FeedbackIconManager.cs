using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeedbackIconManager : MonoBehaviour {

    public GameObject StopPrefab;

    private FeedbackIcon _currentIcon;

    public void ShowStopIcon(bool hideAutomatically = false, float hideDelay = 0.0f)
    {
        if (_currentIcon == null)
        {
            ShowIcon(StopPrefab, hideAutomatically, hideDelay);
        }
        else if (_currentIcon.iconType != IconType.Stop)
        {
            HideCurrentIcon (completion:() => {
                ShowIcon(StopPrefab, hideAutomatically, hideDelay);
            });
        }
        else
        {

        }
    }

    public void HideCurrentIcon(float delay = 0.0f, Action completion = null)
    {
        if (_currentIcon) {
            _currentIcon.ScaleOut(delay, () =>
            {
                Destroy(_currentIcon.gameObject); 
                _currentIcon = null;
                if (completion != null)
                {
                    completion();
                }
            });
        }
    }

    private void ShowIcon(GameObject iconPrefab, bool hideAutomatically = false, float hideDelay = 0.0f)
    {

        var iconGameObject = Instantiate(iconPrefab, transform.position, Quaternion.identity);
        iconGameObject.transform.SetParent(transform);
        _currentIcon = iconGameObject.GetComponent<FeedbackIcon>();
        _currentIcon.ScaleIn(() =>
        {
            if (hideAutomatically)
            {
                HideCurrentIcon(delay: hideDelay);
            }
        });
    }
}
