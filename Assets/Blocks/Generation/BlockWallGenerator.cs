using UnityEngine;

[ExecuteInEditMode]
public class BlockWallGenerator : MonoBehaviour
{
    public const int WallHeight = 15;
    public const int RangeMin = 5;
    public const int RangeMax = 8;

    private BlockColumn _blockColumn;
    private int num_movable;
    private int accumulated;

    void Start()
    {
        _blockColumn = GetComponent<BlockColumn>();
        num_movable = 0;
        accumulated = Random.Range (RangeMin, RangeMax);
    }

    void Update()
    {
        while (0 < _blockColumn.Blocks.Count && _blockColumn.Blocks.Count < WallHeight)
        {
            GameObject newTopBlock;
            var topBlock = _blockColumn.Blocks[_blockColumn.Blocks.Count - 1].gameObject;
            if (Random.Range(1,100) < 5)
            {
                newTopBlock = Instantiate(BlockColumnManager.Instance.BombBlockPrefab);
            } else if (num_movable < accumulated) {
                newTopBlock = Instantiate (BlockColumnManager.Instance.BlockPrefab);
                num_movable++;
            } else {
                newTopBlock = Instantiate (BlockColumnManager.Instance.ImmovableBlockPrefab);
                num_movable = 0;
                accumulated = Random.Range (RangeMin, RangeMax);
            }
            newTopBlock.transform.position = topBlock.transform.position + Vector3.up;
            GetComponent<BlockColumn>().Add(newTopBlock);
            newTopBlock.GetComponent<Block>().MakeFallAfterSlideBlockDelay();
        }
    }
}
