using UnityEngine;

public class BlockRainGenerator : MonoBehaviour
{
    public GameObject BlockPrefab;

    private const float SpawnMinDelay = 2f;
    private const float SpawnMaxDelay = 8f;

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
	        block.transform.position = _blockColumn.SupportPosition + Vector3.up * BlockWallGenerator.WallHeight;
	        _blockColumn.Add(block);
            block.GetComponent<Block>().MakeFallImmediately();
	        SetupNextSpawn();
	    }
	}

    private void SetupNextSpawn()
    {
        _spawnCooldown = Random.Range(SpawnMinDelay, SpawnMaxDelay);
    }
}
