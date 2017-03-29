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
    
    public float SupportBlockHeight { get { return transform.position.y + _supportBoxCollider.center.y; } }

    public GameObject BlockColumnPrefab;
    public GameObject BlockPrefab;
    public GameObject ImmovableBlockPrefab;
    public GameObject BombBlockPrefab;

    //public bool BombBlockEnabled = false;
    //public bool StaticBlockEnabled = false;
    public bool BlockRainEnabled = false;
    public bool CameraScrollEnabled = false;

    private BoxCollider _supportBoxCollider;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        _supportBoxCollider = GetComponent<BoxCollider>();
        _supportBoxCollider.center = new Vector3((Width - 1) / 2f, -1, (Depth - 1) / 2f);

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
                    blockColumnComponent.LayerForNewBlocks = Layers.FloorBlocks;
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
            if (BlockColumns[x, z].Blocks.Count > highestColumn.Blocks.Count)
            {
                highestColumn = BlockColumns[x, z];
            }
        }

        var highestBlock = highestColumn.Blocks[highestColumn.Blocks.Count - 1];
        return highestBlock.transform.position + Vector3.up;
    }

    public void SlideBlockWithEaseInAndLights(GameObject block, Vector3 direction)
    {
        SlideBlockWithEaseIn(block, direction);
        block.GetComponent<Block>().SetOffGlowLight();
    }

    public void SlideBlock(GameObject block, Vector3 direction)
    {
        StartCoroutine(SlideBlockCoroutine(block, direction, () => {
            DetonateIfBomb(block);
        }));
    }

    public void SlideBlockWithEaseIn(GameObject block, Vector3 direction)
    {
        var oldBlockColumn = GetBlockColumnAtLocalPosition(block.transform.parent.localPosition);
        var newBlockColumn = GetBlockColumnAtLocalPosition(block.transform.parent.localPosition + direction);
        var removedBlock = oldBlockColumn.Remove(block.transform.position);

        var oldPosition = removedBlock.transform.position;

        // Documentation for iTween: http://www.pixelplacement.com/itween/documentation.php
        // I made modifications to iTween to accept Lambda expressions for onupdate and oncomplete parameters
        // onupdateinline and oncompleteinline contain said lambda expressions
        Hashtable args = 
            iTween.Hash(
                "from", oldPosition,
                "to", oldPosition + direction,
                "time", SlideBlockDuration,
                "delay", 0,
                "easeType", "easeInCirc",
                "onupdateinline", (Action<object>)(updatedValue => {if (removedBlock != null) removedBlock.transform.position = (Vector3)updatedValue;}),
                "oncompleteinline",(Action<object>)(
                    completeParameters =>
                    {
                        if (removedBlock == null) 
                        {
                            return;
                        }

                        removedBlock.transform.position = oldPosition + direction;
                        removedBlock.transform.position = removedBlock.transform.position.RoundToInt();

                        if (newBlockColumn != null)
                        {
                            newBlockColumn.Add(removedBlock);
                        } else
                        {
                            removedBlock.GetComponent<Block>().MakeFallImmediately();
                        }

                        DetonateIfBomb(removedBlock);
                    }));

        iTween.ValueTo(gameObject, args);
    }

    public void MoveSupportUp()
    {
        _supportBoxCollider.center += Vector3.up;
    }

    private IEnumerator SlideBlockCoroutine(GameObject block, Vector3 direction, Action completion)
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

        if (completion != null)
        {
            completion();
        }
    }

    private void DetonateIfBomb(GameObject block) {
        //if bomb block, set active
        var bombComponent = block.GetComponent<BombBlock>();
        if (bombComponent != null)
        {
            bombComponent.RenderEffectsForTeam = TeamForBlock(block);
            bombComponent.SetBombActive();
        }
    }

    // Will return Team.Purple by default if block is in the wall
    private Team TeamForBlock(GameObject block)
    {
        if (block.transform.position.z == BlueTeamZIndex - 1)
        {
            return Team.Blue;
        }
        else
        {
            return Team.Purple;
        }
    }

    public BlockColumn GetBlockColumnAtLocalPosition(Vector3 localPosition)
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