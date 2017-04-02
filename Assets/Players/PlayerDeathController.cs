using UnityEngine;
using System.Collections;

public class PlayerDeathController : MonoBehaviour
{
    public GameObject DeathParticleSystem;
    public ParticleSystemGroup FallingDeathParticleSystemGroup;

    private Color _particleColorLightTeamBlue   = new Color(0.71372549f, 0.945098039f, 0.960784314f);
    private Color _particleColorTeamBlue        = new Color(0f, 0.901960784f, 0.941176471f);

    private Color _particleColorLightTeamPurple = new Color(1f, 0.670588235f, 0.960784314f);
    private Color _particleColorTeamPurple      = new Color(1f, 0.078431373f, 0.670588235f);

    private Team _playerTeam;

    void Start()
    {
        _playerTeam = GetComponentInParent<PlayerController>().Team;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(Tags.Block))
        {
            var collidingObject = other.gameObject;
            var isUnderBlock = Mathf.Abs(gameObject.transform.position.x - collidingObject.transform.position.x) < 0.5
                               && gameObject.transform.position.y < collidingObject.transform.position.y;
            if (isUnderBlock)
            {
                var lastTouched = other.gameObject.GetComponent<Block>().LastTouchedTeam;
                if (lastTouched != null && lastTouched != _playerTeam)
                {
                    ScoreManager.Instance.IncrementScoreForTeamAndType(lastTouched, ScoreIncrementType.KillPlayerByCrush);
                }
                KillPlayerByCrushing();
            }


        }
        else if (other.CompareTag(Tags.Destructor))
        {
            KillPlayerByFalling();
        }
    }

    public void KillPlayerByCrushing()
    {
        SpawnDeathParticlesAtPosition(gameObject.transform.parent.position + Vector3.up);
        ShakeCameraForTeam(_playerTeam);
        TeamLivesManager.Instance.HandlePlayerDeath(gameObject.transform.parent.gameObject);
    }

    public void KillPlayerByFalling()
    {
        if (_playerTeam == Team.Blue && gameObject.transform.position.z < -1.5f)
        {
            ScoreManager.Instance.IncrementScoreForTeamAndType(Team.Purple, ScoreIncrementType.KillPlayerByPush);
        }
        else if (_playerTeam == Team.Purple && gameObject.transform.position.z > 1.5f)
        {
            ScoreManager.Instance.IncrementScoreForTeamAndType(Team.Blue, ScoreIncrementType.KillPlayerByPush);
        }

        Vector3 forward;

        if (_playerTeam == Team.Blue)
        {
            forward = Vector3.back;
        }
        else
        {
            forward = Vector3.forward;
        }

        SpawnFallDeathParticlesAtPosition(gameObject.transform.position - Vector3.up * 4 + forward);

        ShakeCameraForTeam(_playerTeam);
        TeamLivesManager.Instance.HandlePlayerDeath(gameObject.transform.parent.gameObject);
    }

    private void ShakeCameraForTeam(Team team)
    {
        float time = 0.2f;
        float xMove = 0.15f;
        float yMove = 0.15f;

        CameraController.Instance.ShakeCameraForTeam(team, time, xMove, yMove);
    }

    private void SpawnDeathParticlesAtPosition(Vector3 position)
    {
        GameObject particleObject = Instantiate(DeathParticleSystem, position, Quaternion.identity);
        var system = particleObject.GetComponent<ParticleSystem>();
        var mainModule = particleObject.GetComponent<ParticleSystem>().main;

        mainModule.startColor = MinMaxGradientForTeam(_playerTeam);
        system.Play();

        Destroy(particleObject, particleObject.GetComponent<ParticleSystem>().main.duration);
    }

    private void SpawnFallDeathParticlesAtPosition(Vector3 position)
    {
        var particleObject = Instantiate(FallingDeathParticleSystemGroup, position, Quaternion.identity);
        particleObject.StartParticleSystemAndCleanupForTeam(_playerTeam);
    }

    private ParticleSystem.MinMaxGradient MinMaxGradientForTeam(Team team)
    {
        switch (team)
        {
        case Team.Blue:
            return new ParticleSystem.MinMaxGradient(_particleColorTeamBlue, _particleColorLightTeamBlue);
        case Team.Purple:
            return new ParticleSystem.MinMaxGradient(_particleColorTeamPurple, _particleColorLightTeamPurple);
        default:
            return new ParticleSystem.MinMaxGradient(Color.black, Color.white);
        }
    }
}
