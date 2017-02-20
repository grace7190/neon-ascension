using UnityEngine;

public class BlockRainGenerator : MonoBehaviour
{
    public GameObject BlockPrefab;

    private const float SpawnMinDelay = 2f;
    private const float SpawnMaxDelay = 8f;
    private const float SpawnRate = 0.6f; //higher = more blocks

    private float _spawnCooldown;
    private BlockColumn _blockColumn;

    void Start()
    {
        _blockColumn = GetComponent<BlockColumn>();

        SetupNextSpawn();
    }

	void Update ()
	{
	    _spawnCooldown -= Time.deltaTime;
	    if (_spawnCooldown <= 0 && transform.childCount < BlockWallGenerator.WallHeight)
	    {
	        var block = Instantiate(BlockPrefab);
	        block.transform.position = _blockColumn.SupportPosition + Vector3.up * (BlockWallGenerator.WallHeight-1);
            block.transform.position = block.transform.position.RoundToInt();
            _blockColumn.Add(block);
            block.GetComponent<Block>().MakeFallImmediately();
	        SetupNextSpawn();
	    }
	}

    private void SetupNextSpawn()
    {
        _spawnCooldown = (Random.Range(SpawnMinDelay, SpawnMaxDelay) + Random.Range(SpawnMinDelay, SpawnMaxDelay)) / SpawnRate;
    }
}
