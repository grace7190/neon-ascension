using UnityEngine;

[ExecuteInEditMode]
public class BlockWallGenerator : MonoBehaviour
{
    public const int WallHeight = 15;

    private BlockColumn _blockColumn;

    void Start()
    {
        _blockColumn = GetComponent<BlockColumn>();
    }

    void Update()
    {
        while (_blockColumn.Blocks.Count < WallHeight)
        {
            var topBlock = _blockColumn.Blocks[_blockColumn.Blocks.Count - 1].gameObject;
            var newTopBlock = Instantiate(topBlock);
            newTopBlock.transform.position = topBlock.transform.position + Vector3.up;
            GetComponent<BlockColumn>().Add(newTopBlock);
            newTopBlock.GetComponent<Block>().MakeFallAfterSlideBlockDelay();
        }
    }
}
