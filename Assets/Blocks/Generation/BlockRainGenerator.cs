using UnityEngine;

public class BlockRainGenerator : MonoBehaviour
{
    private const float SpawnMinDelay = 2f;
    private const float SpawnMaxDelay = 8f;

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
	        var bottomBlock = transform.GetChild(0).gameObject;
            var block = Instantiate(bottomBlock);
	        block.transform.position = bottomBlock.transform.position + Vector3.up * BlockWallGenerator.WallHeight;
	        GetComponent<BlockColumn>().Add(block);
            block.GetComponent<Block>().MakeFall();
	        SetupNextSpawn();
	    }
	}

    private void SetupNextSpawn()
    {
        _spawnCooldown = Random.Range(SpawnMinDelay, SpawnMaxDelay);
    }
}
