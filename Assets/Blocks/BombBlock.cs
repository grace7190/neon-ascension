using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class BombBlock : Block
{

    public static readonly Color TickColor = new Color(.952941176f, .545098039f, .203921569f);

    public Material BombActiveMaterial;
    public const int BombTicks = 5;
	public AudioClip ticktok_clip;
	public AudioClip explosion_clip;

    private float blinkTimes;
    private bool _shouldDetonate;
	private AudioSource audio;

    // Use this for initialization
    void Start () {
        _shouldDetonate = false;
        _rigidbody = GetComponent<Rigidbody>();
		audio = GetComponent<AudioSource> ();
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
        
        Vector3 blockPosition;
        BlockColumn col;
        blockPosition = gameObject.transform.parent.localPosition;

		audio.clip = explosion_clip;
		audio.Play ();

        // Use Physics.OverlapBox to find everything in radius of box
        Collider[] objectsInRange = Physics.OverlapBox(gameObject.transform.position, new Vector3(1.0f, 1.0f, 1.0f));
        foreach (Collider collider in objectsInRange)
        {

            if (collider.gameObject.tag == "Block" && collider.gameObject != gameObject )
            {
                blockPosition = collider.gameObject.transform.parent.localPosition;
                col = BlockColumnManager.Instance.GetBlockColumnAtLocalPosition(blockPosition);

                // Push blocks off
                collider.attachedRigidbody.isKinematic = false;
                if (blockPosition.z == BlockColumnManager.WallZIndex)
                {
                    BlockColumnManager.Instance.destroyBlock(col, collider.gameObject.transform.position);
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
            if (collider.gameObject.tag == "Player")
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

        blockPosition = gameObject.transform.parent.localPosition;
        col = BlockColumnManager.Instance.GetBlockColumnAtLocalPosition(blockPosition);
        BlockColumnManager.Instance.destroyBlock(col, gameObject.transform.position);
    }

    private IEnumerator AnimateDetonatingCoroutine(int bombTicks, Action completion = null)
    {
        float flashDurationNormal = 0.3f;
        float flashDurationDetonation = 0.1f;
        float changeTime = 0.01f;
		audio.clip = ticktok_clip;
		audio.Play ();

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
