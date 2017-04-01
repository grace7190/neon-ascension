using System;
using System.Collections;
using UnityEngine;

public class BombBlock : Block
{
    public static readonly Color TickColor = new Color(.952941176f, .545098039f, .203921569f);
    public static readonly Color BlockDestructionColor = Color.white;

    public ParticleSystemGroup ExplosionParticlesGroup;
    public GameObject BlockDestructionParticles;

    public Material BombActiveMaterial;
    public const int BombTicks = 10;
    public AudioClip TicktokClip;
    public AudioClip ExplosionClip;

    public Team RenderEffectsForTeam;

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

        // Render effects
        ShakeCamera();
        StartExplosionParticlesAtPosition(transform.position);

        // Use Physics.OverlapBox to find everything in radius of box
        Collider[] objectsInRange = Physics.OverlapBox(gameObject.transform.position, new Vector3(1.0f, 1.0f, 1.0f));
        foreach (Collider collider in objectsInRange)
        {
            if (collider.gameObject.tag == Tags.Block && collider.gameObject != gameObject)
            {
                StartBlockDestructionParticlesAtPosition(collider.transform.position, collider.gameObject.GetComponent<Block>().BaseColor);

                if (collider.transform.parent != null)
                {
                    blockPosition = collider.gameObject.transform.parent.localPosition;
                    col = BlockColumnManager.Instance.GetBlockColumnAtLocalPosition(blockPosition);
                    col.DestroyBlockAtPosition(collider.gameObject.transform.position);
                }
                else
                {
                    Destroy(collider.gameObject);
                }

            }

            if (collider.gameObject.tag == Tags.Player)
            {
                collider.gameObject.GetComponentInChildren<PlayerDeathController>().KillPlayerByCrushing();
                Team team = collider.gameObject.GetComponent<PlayerController>().Team;

                //add score to opponent when player dies to bomb that opponent pushed
                if (team != LastTouchedTeam)
                {
                    ScoreManager.Instance.IncrementScoreForTeamAndType(LastTouchedTeam, ScoreIncrementType.KillPlayerByBomb);
                }
            }
        }

        StartCoroutine(CleanupCoroutine());
    }

    private void ShakeCamera()
    {
        float time = 0.2f;
        float xMove = 0.15f;
        float yMove = 0.15f;

        CameraController.Instance.ShakeCameraForTeam(RenderEffectsForTeam, time, xMove, yMove);
    }

    private void StartExplosionParticlesAtPosition(Vector3 position)
    {
         Quaternion rotation;

        if (RenderEffectsForTeam == Team.Purple)
        {
            rotation = Quaternion.AngleAxis(30, Vector3.up);
        }
        else 
        {
            rotation = Quaternion.identity;
        }

        var particleObject = Instantiate(ExplosionParticlesGroup, position, rotation);
        particleObject.StartParticleSystemAndCleanupForTeam(RenderEffectsForTeam);
    }

    private void StartBlockDestructionParticlesAtPosition(Vector3 position, Color themedDestructionColor)
    {
        GameObject particleObject = Instantiate(BlockDestructionParticles, position, Quaternion.identity);
        var system = particleObject.GetComponent<ParticleSystem>();
        var mainModule = particleObject.GetComponent<ParticleSystem>().main;

        mainModule.startColor = MinMaxGradientWithThemedDestructionColor(themedDestructionColor);
        system.Play();

        Destroy(particleObject, particleObject.GetComponent<ParticleSystem>().main.duration);
    }

    private ParticleSystem.MinMaxGradient MinMaxGradientWithThemedDestructionColor(Color themedColor)
    {
        return new ParticleSystem.MinMaxGradient(BlockDestructionColor, themedColor);
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

        ChangeColor(TickColor, changeTime, completion:completion);
    }
}
