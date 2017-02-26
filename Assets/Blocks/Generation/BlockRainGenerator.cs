using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class BlockRainGenerator : MonoBehaviour
{
    public GameObject BlockPrefab;

    private const float SpawnMinDelay = 1f;
    private const float SpawnMaxDelay = 2f;

    private float _spawnCooldown;

    void Start()
    {
        SetupNextSpawn();
    }

	void Update ()
	{
        _spawnCooldown -= Time.deltaTime;
	    if (_spawnCooldown <= 0)
	    {
	        TrySpawn(BlockColumnManager.BlueTeamZIndex);
	        TrySpawn(BlockColumnManager.PurpleTeamZIndex);
	    }
	}

    private void TrySpawn(int zIndex)
    {
        var openBlockColumns = new List<BlockColumn>();
        for (var x = 0; x < BlockColumnManager.Width; x++)
        {
            var blockColumn = BlockColumnManager.Instance.BlockColumns[x, zIndex];
            var isColumnTopOpen = blockColumn.transform.childCount == 0 ||
                GetBlockSpawnPosition(blockColumn).y - blockColumn.transform.GetChild(blockColumn.transform.childCount - 1).transform.position.y >= 1;
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
            block.GetComponent<Block>().MakeFallImmediately();
            SetupNextSpawn();
        }
    }

    private void SetupNextSpawn()
    {
        _spawnCooldown = Random.Range(SpawnMinDelay, SpawnMaxDelay);
    }

    private Vector3 GetBlockSpawnPosition(BlockColumn blockColumn)
    {
	    return blockColumn.SupportPosition + Vector3.up * (BlockWallGenerator.WallHeight - 1);
    }
}
