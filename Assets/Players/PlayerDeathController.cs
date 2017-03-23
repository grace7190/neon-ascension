using UnityEngine;

public class PlayerDeathController : MonoBehaviour
{
    public GameObject DeathParticleSystem;

    private Color _particleColorLightTeamBlue   = new Color(0.71372549f, 0.945098039f, 0.960784314f);
    private Color _particleColorTeamBlue        = new Color(0f, 0.901960784f, 0.941176471f);

    private Color _particleColorLightTeamPurple = new Color(1f, 0.670588235f, 0.960784314f);
    private Color _particleColorTeamPurple      = new Color(1f, 0.078431373f, 0.670588235f);


    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(Tags.Block))
        {
            var collidingObject = other.gameObject;
            var isUnderBlock = Mathf.Abs(gameObject.transform.position.x - collidingObject.transform.position.x) < 0.5
                               && gameObject.transform.position.y < collidingObject.transform.position.y;
            if (isUnderBlock)
            {
                SpawnDeathParticlesAtPosition(gameObject.transform.parent.position);
                TeamLivesManager.Instance.HandlePlayerDeath(gameObject.transform.parent.gameObject);
            }
        }
        else if (other.CompareTag(Tags.Destructor))
        {
            TeamLivesManager.Instance.HandlePlayerDeath(gameObject.transform.parent.gameObject);
        }
    }

    private void SpawnDeathParticlesAtPosition(Vector3 position)
    {
        GameObject particleObject = Instantiate(DeathParticleSystem, position, Quaternion.identity);
        var system = particleObject.GetComponent<ParticleSystem>();
        var mainModule = particleObject.GetComponent<ParticleSystem>().main;

        mainModule.startColor = MinMaxGradientForTeam(GetComponentInParent<PlayerController>().Team);
        system.Play();

        Destroy(particleObject, particleObject.GetComponent<ParticleSystem>().main.duration);
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
