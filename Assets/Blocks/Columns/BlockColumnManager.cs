using System;
using System.Collections;
using UnityEngine;

[ExecuteInEditMode]
public class BlockColumnManager : MonoBehaviour
{
    public const float SlideBlockDuration = 0.25f;

    public static BlockColumnManager Instance;

    public GameObject BlockColumnPrefab;
    public GameObject BlockPrefab;

    private const int Depth = 3;
    private const int Width = 10;
    private const int WallIndex = 1;

    private readonly BlockColumn[,] _blockColumns = new BlockColumn[Width, Depth];

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        while (transform.childCount > 0)
        {
            var child = transform.GetChild(0);
            DestroyImmediate(child.gameObject);
        }

        transform.position = new Vector3(-Width / 2f, 0, -(Depth - 1) / 2f);
        for (var x = 0; x < Width; x++)
        {
            for (var z = 0; z < Depth; z++)
            {
                var blockColumn = Instantiate(BlockColumnPrefab);
                blockColumn.transform.SetParent(transform);
                blockColumn.transform.localPosition = new Vector3(x, 0, z);
                var blockColumnComponent = blockColumn.GetComponent<BlockColumn>();
                _blockColumns[x, z] = blockColumnComponent;
                    
                var block = Instantiate(BlockPrefab);
                block.transform.position = transform.position + new Vector3(x, 0, z);
                blockColumnComponent.Add(block);
                block.GetComponent<Block>().MakeFall();

                if (z == WallIndex)
                {
                    blockColumnComponent.IsWall = true;
                }
            }
        }
    }

    public void SlideBlock(GameObject block, Vector3 direction)
    {
        StartCoroutine(SlideBlockCoroutine(block, direction));
    }

    private IEnumerator SlideBlockCoroutine(GameObject block, Vector3 direction)
    {
        var oldBlockColumn = GetBlockColumnAtLocalPosition(block.transform.parent.localPosition);
        var newBlockColumn = GetBlockColumnAtLocalPosition(block.transform.parent.localPosition + direction);

        var removedBlock = oldBlockColumn.Remove(block.transform.position);

        var t = 0f;
        var oldPosition = removedBlock.transform.position;
        while (t <= SlideBlockDuration)
        {
            removedBlock.transform.position = Vector3.Lerp(oldPosition, oldPosition + direction, t / SlideBlockDuration);
            yield return new WaitForEndOfFrame();
            t += Time.deltaTime;
        }
        removedBlock.transform.position = oldPosition + direction;
        removedBlock.transform.position = removedBlock.transform.position.RoundToInt();

        if (newBlockColumn != null)
        {
            newBlockColumn.Add(removedBlock);
        }
    }

    private BlockColumn GetBlockColumnAtLocalPosition(Vector3 localPosition)
    {
        try
        {
            return _blockColumns[Mathf.RoundToInt(localPosition.x), Mathf.RoundToInt(localPosition.z)];
        }
        catch (IndexOutOfRangeException)
        {
            return null;
        }
    }
}