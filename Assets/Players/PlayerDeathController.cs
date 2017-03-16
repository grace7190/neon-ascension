using UnityEngine;

public class PlayerDeathController : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(Tags.Block))
        {
            var collidingObject = other.gameObject;
            var isUnderBlock = gameObject.transform.position.x - collidingObject.transform.position.x < 0.5
                               && gameObject.transform.position.y < collidingObject.transform.position.y
                               && other.GetComponent<Rigidbody>().velocity.y < 0;
            if (isUnderBlock)
            {
                TeamLivesManager.Instance.HandlePlayerDeath(gameObject.transform.parent.gameObject);
            }
        }
        else if (other.CompareTag(Tags.Destructor))
        {
            TeamLivesManager.Instance.HandlePlayerDeath(gameObject.transform.parent.gameObject);
        }
    }
}
