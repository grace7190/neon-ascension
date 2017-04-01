using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreEmitter : MonoBehaviour {

    public GameObject TextGroupPrefab;
    public int TextCameraLayer;

     void Start () {
        // Billboard
        transform.rotation = Quaternion.identity;
     }

    void LateUpdate() {
        // Billboard
        transform.rotation = Quaternion.identity;
    }

    public void EmitScoreWithDescription(int score, string description)
    {
        var scoreTextGroupObject = Instantiate(TextGroupPrefab, transform.position, Quaternion.identity);
        var scoreTextGroup = scoreTextGroupObject.GetComponent<ScoreTextGroup>();
        scoreTextGroup.SetCameraLayer(TextCameraLayer);
        scoreTextGroup.SetScoreAndDescription(score, description);
        scoreTextGroup.StartAnimation();
    }

}
