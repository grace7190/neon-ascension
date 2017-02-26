using UnityEngine;

public class Destructor : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(Tags.Block))
        {
            var blockColumn = other.GetComponentInParent<BlockColumn>();
            if (blockColumn != null)
            {
                blockColumn.Remove(other.transform.position);
            }

            if (BlockColumnManager.Instance.SupportBlockHeight - transform.position.y < 0.5)
            {
                BlockColumnManager.Instance.MoveSupportUp();
            }

            Destroy(other.gameObject);
        }
    }
}
