using UnityEngine;

[ExecuteInEditMode]
public class BlockWallGenerator : MonoBehaviour
{
    public const int WallHeight = 15;
    private int ConsecutiveMovableBlockMin;// = 5;
    private int ConsecutiveMovableBlockMax;// = 12;

    public bool IsInitialized { get; private set; }

    //public bool BombBlockEnabled = false;
    //public bool StaticBlockEnabled = false; 

    private BlockColumn _blockColumn;
    private int _consecutiveMovableBlocks;
    private int _consecutiveMovableBlocksBeforeImmovableBlock;

    private float timeToScaleDifficulty = 120.0f;

    void Start()
    {
        IsInitialized = false;
        _blockColumn = GetComponent<BlockColumn>();
        _consecutiveMovableBlocks = 0;
        ConsecutiveMovableBlockMin = 10;
        ConsecutiveMovableBlockMax = 30;
        _consecutiveMovableBlocksBeforeImmovableBlock = Random.Range (ConsecutiveMovableBlockMin, ConsecutiveMovableBlockMax);
    }

    void Update()
    {
        while (0 < _blockColumn.Blocks.Count && _blockColumn.Blocks.Count < WallHeight)
        {
            var topBlock = _blockColumn.Blocks[_blockColumn.Blocks.Count - 1].gameObject;
            var bottomBlock = _blockColumn.Blocks[0].gameObject;
            
            var isTopOfWallOpen = topBlock.transform.position.y <= bottomBlock.transform.position.y + WallHeight - 1;
            if (!isTopOfWallOpen)
            {
                return;
            }

            GameObject newTopBlock;
            var maxChance = Mathf.Max(25, Mathf.RoundToInt(timeToScaleDifficulty) - Mathf.RoundToInt(Time.time));
            if (_blockColumn.Blocks.Count > 9 && Random.Range(1, maxChance) <= 3)
            {
                newTopBlock = Instantiate(BlockColumnManager.Instance.BombBlockPrefab);
            } 
            else if (_blockColumn.Blocks.Count > 9 && _consecutiveMovableBlocks > _consecutiveMovableBlocksBeforeImmovableBlock)
            {
                newTopBlock = Instantiate(BlockColumnManager.Instance.ImmovableBlockPrefab);
                _consecutiveMovableBlocks = 0;
                _consecutiveMovableBlocksBeforeImmovableBlock = Random.Range(ConsecutiveMovableBlockMin, ConsecutiveMovableBlockMax);
                //decrease over time to half of original min/max
                _consecutiveMovableBlocksBeforeImmovableBlock *= Mathf.RoundToInt(Mathf.Max(0.5f, (timeToScaleDifficulty*2 - Time.time) / timeToScaleDifficulty*2));
            }
            else
            {
                newTopBlock = Instantiate(BlockColumnManager.Instance.BlockPrefab);
                _consecutiveMovableBlocks++;
            }

            newTopBlock.transform.position = bottomBlock.transform.position + Vector3.up * WallHeight;
            _blockColumn.Add(newTopBlock);

            if (IsInitialized)
            {
                newTopBlock.GetComponent<Block>().MakeFallAfterSlideBlockDelay();
            }
            else
            {
                newTopBlock.GetComponent<Block>().MakeFallAfterDelay(Random.Range(0.0f, 0.1f));
            }

            if (_blockColumn.Blocks.Count == WallHeight)
            {
                IsInitialized = true;
                GameController.Instance.IsStarted = true;
            }
        }
    }
}
