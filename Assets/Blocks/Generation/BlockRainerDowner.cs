using UnityEngine;

public class BlockRainerDowner : MonoBehaviour
{
    public int Size;
    public Vector3 OffsetDirection;
    public float SpawnMinDelay;
    public float SpawnMaxDelay;

    private int _nextOffset;
    private float _spawnCooldown;
    private BlockGenerator _blockGenerator;

    void Start()
    {
        _blockGenerator = GetComponent<BlockGenerator>();
        SetupNextSpawn();
    }

	void Update ()
	{
	    _spawnCooldown -= Time.deltaTime;
	    if (_spawnCooldown <= 0)
	    {
            var block = _blockGenerator.MakeBlock();
	        block.transform.position = transform.position + OffsetDirection * _nextOffset;
	        SetupNextSpawn();
	    }
	}

    private void SetupNextSpawn()
    {
        _nextOffset = Random.Range(0, Size);
        _spawnCooldown = Random.Range(SpawnMinDelay, SpawnMaxDelay);
    }
}
