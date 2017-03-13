using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class BombBlock : Block
{

    public static readonly Color TickColor = new Color(.952941176f, .545098039f, .203921569f);

    public Material BombActiveMaterial;
    public const int BombTicks = 10;
    public AudioClip TicktokClip;
    public AudioClip ExplosionClip;

    private float _blinkTimes;
    private bool _shouldDetonate;
    private AudioSource _audio;

    // Use this for initialization
    void Start ()
    {
        _shouldDetonate = false;
        _rigidbody = GetComponent<Rigidbody>();
        _audio = GetComponent<AudioSource> ();
    }

    void Update()
    {
        // If not falling, and should detonate, start detonation
        if (_shouldDetonate && _rigidbody.velocity.y == 0) {
            _shouldDetonate = false;
            StartDetonation();
        }
    }

    public void SetBombActive()
    {
        _shouldDetonate = true;
    }

    private void StartDetonation()
    {
        AnimateDetonating(completion:() =>
        {
            Detonate();
        });
    }

    private void AnimateDetonating(Action completion)
    {
        StartCoroutine(AnimateDetonatingCoroutine(BombTicks, completion));
    }

    private void Detonate()
    {
        
        Vector3 blockPosition;
        BlockColumn col;
        blockPosition = gameObject.transform.parent.localPosition;

        _audio.clip = ExplosionClip;
        _audio.Play ();

        // Remove bomb from column
        blockPosition = gameObject.transform.parent.localPosition;
        col = BlockColumnManager.Instance.GetBlockColumnAtLocalPosition(blockPosition);
        col.Remove(gameObject.transform.position);

        // Use Physics.OverlapBox to find everything in radius of box
        Collider[] objectsInRange = Physics.OverlapBox(gameObject.transform.position, new Vector3(1.0f, 1.0f, 1.0f));
        foreach (Collider collider in objectsInRange)
        {

            if (collider.gameObject.tag == Tags.Block && collider.gameObject != gameObject)
            {
                blockPosition = collider.gameObject.transform.parent.localPosition;
                col = BlockColumnManager.Instance.GetBlockColumnAtLocalPosition(blockPosition);

                // Push blocks off
                collider.attachedRigidbody.isKinematic = false;
                if (blockPosition.z == BlockColumnManager.WallZIndex)
                {
                    col.DestroyBlockAtPosition(collider.gameObject.transform.position);
                }
                else if (blockPosition.z == BlockColumnManager.PurpleTeamZIndex)
                {
                    BlockColumnManager.Instance.SlideBlock(collider.gameObject, Vector3.forward);
                }
                else if (blockPosition.z == BlockColumnManager.BlueTeamZIndex)
                {
                    BlockColumnManager.Instance.SlideBlock(collider.gameObject, Vector3.back);
                }
            }

            // Push player off
            if (collider.gameObject.tag == Tags.Player)
            {
                PlayerController pc = collider.gameObject.GetComponent<PlayerController>();

                if (pc.Team == Team.Purple)
                {
                    pc.transform.rotation = Quaternion.identity;
                    collider.GetComponent<Rigidbody>().AddForce(Vector3.forward * 100);
                }
                else
                {
                    pc.transform.rotation = Quaternion.identity;
                    pc.GetComponent<Rigidbody>().AddForce(Vector3.back * 100);
                }
            }
        }

        StartCoroutine(CleanupCoroutine());
    }

    private IEnumerator CleanupCoroutine()
    {

        GetComponent<Collider>().enabled = false;
        GetComponent<Renderer>().enabled = false;
        _rigidbody.isKinematic = true;

        // Delete after sound finishes
        while(_audio.isPlaying)
        {
            yield return null;
        }

        Destroy(gameObject);
    }

    private IEnumerator AnimateDetonatingCoroutine(int bombTicks, Action completion = null)
    {
        float flashDurationNormal = 0.2f;
        float flashDurationDetonation = 0.1f;
        float changeTime = 0.01f;
        _audio.clip = TicktokClip;

        for (int i = 0; i < bombTicks; i++)
        {
            float elapsed_time = 0.0f;
            ChangeColor(TickColor, changeTime);
            _audio.Play ();
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

            if (flashDurationNormal > 0.01f)
            {
                flashDurationNormal -= flashDurationNormal * 0.5f;
            }
        }

        ChangeColor(TickColor, changeTime, completion);
    }
}
