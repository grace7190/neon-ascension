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
                blockColumn.MoveSupportUp();
            }

            Destroy(other.gameObject);
        }
    }
}
