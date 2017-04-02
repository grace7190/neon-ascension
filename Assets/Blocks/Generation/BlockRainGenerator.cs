using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class BlockRainGenerator : MonoBehaviour
{
    public GameObject BlockPrefab;
    public const float FallDelay = .3f;

    private const float SpawnMinDelay = 1.5f;
    private const float SpawnMaxDelay = 3.5f;
    private float timeToScaleSpawnRate = 120.0f;

    private float _spawnCooldown;

    void Start()
    {
        SetupNextSpawn();
    }

	void Update ()
	{
	    if (GameController.Instance.IsStarted)
	    {
	        _spawnCooldown -= Time.deltaTime;
	        if (BlockColumnManager.Instance.BlockRainEnabled && _spawnCooldown <= 0)
	        {
	            TrySpawn(BlockColumnManager.BlueTeamZIndex);
	            TrySpawn(BlockColumnManager.PurpleTeamZIndex);
	        }
	    }
	}

    private void TrySpawn(int zIndex)
    {
        var openBlockColumns = new List<BlockColumn>();
        for (var x = 0; x < BlockColumnManager.Width; x++)
        {
            var blockColumn = BlockColumnManager.Instance.BlockColumns[x, zIndex];
            var isColumnTopOpen = blockColumn.Blocks.Count == 0 ||
                GetBlockSpawnPosition(blockColumn).y - blockColumn.Blocks[blockColumn.Blocks.Count - 1].transform.position.y >= 1;
            if (isColumnTopOpen)
            {
                openBlockColumns.Add(blockColumn);
            }
        }

        if (openBlockColumns.Count > 0)
        {
            var i = Random.Range(0, openBlockColumns.Count);
            var openBlockColumn = openBlockColumns[i];
            var block = Instantiate(BlockPrefab);
            block.transform.position = GetBlockSpawnPosition(openBlockColumn);
            block.transform.position = block.transform.position.RoundToInt();
            openBlockColumn.Add(block);
            block.GetComponent<Block>().MakeFallAfterDelay(FallDelay);
            SetupNextSpawn();
        }
    }

    private void SetupNextSpawn()
    {
        _spawnCooldown = Random.Range(SpawnMinDelay, SpawnMaxDelay) * Mathf.Max(0.4f, (timeToScaleSpawnRate * 2 - Time.time) / timeToScaleSpawnRate * 2);
    }

    private Vector3 GetBlockSpawnPosition(BlockColumn blockColumn)
    {
	    return blockColumn.transform.position + Vector3.up * (BlockColumnManager.Instance.SupportBlockHeight + BlockWallGenerator.WallHeight);
    }
}
