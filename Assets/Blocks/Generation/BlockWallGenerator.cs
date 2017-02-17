using UnityEngine;

[ExecuteInEditMode]
public class BlockWallGenerator : MonoBehaviour
{
    public const int WallHeight = 15;

    void Update()
    {
        while (transform.childCount < WallHeight)
        {
            var topBlock = transform.GetChild(transform.childCount - 1).gameObject;
            var newTopBlock = Instantiate(topBlock);
            newTopBlock.transform.position = topBlock.transform.position + Vector3.up;
            GetComponent<BlockColumn>().Add(newTopBlock);
            newTopBlock.GetComponent<Block>().MakeFall();
        }
    }
}
