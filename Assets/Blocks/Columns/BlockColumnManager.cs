using System;
using System.Collections;
using UnityEngine;

[ExecuteInEditMode]
public class BlockColumnManager : MonoBehaviour
{
    public const int Depth = 3;
    public const int Width = 10;
    public const int WallZIndex = 1;
    public const int BlueTeamZIndex = 0;
    public const int PurpleTeamZIndex = 2;
    public const float SlideBlockDuration = 0.25f;

    public static BlockColumnManager Instance;

    public readonly BlockColumn[,] BlockColumns = new BlockColumn[Width, Depth];
    
    public GameObject BlockColumnPrefab;
    public GameObject BlockPrefab;


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
                blockColumnComponent.Initialize();
                BlockColumns[x, z] = blockColumnComponent;
                    
                if (z == WallZIndex)
                {
                    blockColumnComponent.gameObject.AddComponent(typeof(BlockWallGenerator));
                    blockColumnComponent.BaseColor = Block.NeutralColor;
                }
                else
                {
                    blockColumnComponent.BaseColor = z == BlueTeamZIndex ? Block.BlueColor : Block.PurpleColor;
                }

                var block = Instantiate(BlockPrefab);
                block.transform.position = transform.position + new Vector3(x, 0, z);
                blockColumnComponent.Add(block);
                var blockComponent = block.GetComponent<Block>();
                blockComponent.Initialize();
                blockComponent.MakeFallImmediately();
            }
        }
    }

    public Vector3 GetRespawnPoint(Team team)
    {
        var z = team == Team.Blue ? BlueTeamZIndex : PurpleTeamZIndex;
        var highestColumn = BlockColumns[0, z];
        for (var x = 1; x < Width; x++)
        {
            if (BlockColumns[x, z].transform.childCount > highestColumn.transform.childCount)
            {
                highestColumn = BlockColumns[x, z];
            }
        }

        var highestBlock = highestColumn.transform.GetChild(highestColumn.transform.childCount - 1);
        return highestBlock.position + Vector3.up;
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
            return BlockColumns[Mathf.RoundToInt(localPosition.x), Mathf.RoundToInt(localPosition.z)];
        }
        catch (IndexOutOfRangeException)
        {
            return null;
        }
    }
}