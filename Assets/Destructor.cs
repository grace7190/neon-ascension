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
            else {
                // This block is no longer in the game zone, let it fall
                return;
            }

            var thisCollider = GetComponent<Collider>();

            if (BlockColumnManager.Instance.SupportBlockHeight - transform.position.y < thisCollider.bounds.size.y / 2)
            {
                BlockColumnManager.Instance.MoveSupportUp();
            }

            Block block = other.gameObject.GetComponent<Block>();
            // Since the Support will be directly over this block, we need to make it non-collidable or else it will shift
            block.gameObject.layer = Layers.IgnoreColumnSupport;
            block.AnimateDeletion();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(Tags.Block))
        {
            Destroy(other.gameObject);
        }
    }
}
