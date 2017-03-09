using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class BombBlock : Block
{

    public static readonly Color TickColor = new Color(.952941176f, .545098039f, .203921569f);

    public Material BombActiveMaterial;
    public const int BombTicks = 5;

    private float blinkTimes;
    private bool _shouldDetonate;

    // Use this for initialization
    void Start () {
        _shouldDetonate = false;
        _rigidbody = GetComponent<Rigidbody>();
    }

    void Update() {
        // If not falling, and should detonate, start detonation
        if (_shouldDetonate && _rigidbody.velocity.y == 0) {
            _shouldDetonate = false;
            StartDetonation();
        }
    }

    public void SetBombActive() {
        _shouldDetonate = true;
    }

    private void StartDetonation() {
        AnimateDetonating(completion:() =>
        {
            Detonate();
        });
    }

    private void AnimateDetonating(Action completion)
    {
        StartCoroutine(AnimateDetonatingCoroutine(BombTicks, completion));
    }

    private void Detonate() {

        Debug.Log("tsss....💣 boom!!🔥🔥");

        // Use BlockColumn.Remove instead
        Destroy(gameObject);
        // TODO
        // Use Physics.OverlapBox to find everything in radius of box
        // Look at PlayerController, to see how Casts, Layers work if needed
        // https://docs.unity3d.com/ScriptReference/Physics.OverlapBox.html

    }

    private IEnumerator AnimateDetonatingCoroutine(int bombTicks, Action completion = null)
    {
        float flashDurationNormal = 0.3f;
        float flashDurationDetonation = 0.1f;
        float changeTime = 0.01f;

        for (int i = 0; i < bombTicks; i++)
        {
            float elapsed_time = 0.0f;
            ChangeColor(TickColor, changeTime);

            // Yield till end of frame to ensure ChangeColor has finished
            while (elapsed_time < changeTime + flashDurationDetonation)
            {
                yield return new WaitForEndOfFrame();
                elapsed_time += Time.deltaTime;
            }

            ChangeColor(LockedColor, changeTime);
            elapsed_time = 0;

            // Yield till end of frame to ensure ChangeColor has finished
            while (elapsed_time < changeTime + flashDurationNormal)
            {
                yield return new WaitForEndOfFrame();
                elapsed_time += Time.deltaTime;
            }

            if (flashDurationNormal > 0.1f)
            {
                flashDurationNormal -= flashDurationNormal * 0.5f;
            }
        }

        ChangeColor(TickColor, changeTime, completion);
    }
}
