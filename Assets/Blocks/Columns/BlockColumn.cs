﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class BlockColumn : MonoBehaviour
{
    /// <summary>
    /// A list of blocks in the column in ascending order by y-position. </summary>
    public readonly List<Block> Blocks = new List<Block>();

    // Blocks that are added to this block column will be set to this layer
    public int LayerForNewBlocks = Layers.Solid;

    public Color BaseColor;

    private GameObject _blockFallIndicator;


    void Start()
    {
        Initialize();
        StartCoroutine(BlockFallCoroutine(true));
    }

    void Update()
    {
        var isCompacted = true;
        for (var i = 1; i < Blocks.Count; i++)
        {
            var currBlock = Blocks[i];
            var prevBlock = Blocks[i - 1];
            if (currBlock.transform.position.y - prevBlock.transform.position.y > 2)
            {
                isCompacted = false;
                _blockFallIndicator.transform.position = prevBlock.transform.position + Vector3.up;
            }
        }

        _blockFallIndicator.SetActive(!isCompacted && Blocks.Count > 0);
    }

    public void Initialize()
    {
        _blockFallIndicator = transform.FindChild("BlockFallIndicator").gameObject;
    }

    private IEnumerator BlockFallCoroutine(bool repeat)
    {
        while (repeat)
        {
            for (var i = 0; i < Blocks.Count; i++)
            {
                if (i == 0)
                {
                    if (Blocks[i].transform.position.y > BlockColumnManager.Instance.SupportBlockHeight + 1.0 &&
                        !Blocks[i].DidMakeFall)
                        Blocks[i].MakeFallImmediately();
                }
                else
                {
                    if (Blocks[i].transform.position.y - Blocks[i - 1].transform.position.y > 1.1)
                    {
                        if (!Blocks[i].DidMakeFall)
                        {
                            Blocks[i].MakeFallImmediately();
                        }
                    }
                }
            }

            yield return new WaitForSeconds(0.2f);
        }
    }

    public GameObject Remove(Vector3 position)
    {
        Block removedBlock = null;
        foreach (var block in Blocks)
        {
            if (removedBlock == null && block.transform.position == position)
            {
                removedBlock = block;
                removedBlock.MakeFallAfterSlideBlockDelay();
            }
            else if (removedBlock != null)
            {
                var isRemovedBlockLowest = Mathf.Approximately(removedBlock.transform.position.y, BlockColumnManager.Instance.SupportBlockHeight + 1);
                if (!isRemovedBlockLowest && !block.DidMakeFall)
                {
                    block.MakeFallAfterSlideBlockDelay();
                }
            }
        }

        if (removedBlock == null)
        {
            throw new ArgumentException(string.Format("No block at {0} in column at {1}", position,
                transform.position));
        }

        removedBlock.transform.SetParent(null);
        Blocks.Remove(removedBlock);

        return removedBlock.gameObject;
    }

    public bool DestroyBlockAtPosition(Vector3 blockPosition)
    {
        try
        {
            var destroyedBlock = Remove(blockPosition);
            Destroy(destroyedBlock);
        }
        catch (ArgumentException)
        {
            Debug.Log(String.Format("no block to Destroy at {0}", blockPosition));
            return false;
        }

        return true;
    }

    public void Add(GameObject block)
    {
        var blockComponent = block.GetComponent<Block>();
        block.layer = LayerForNewBlocks;

        if (!blockComponent.FixedBaseColor)
        {
            blockComponent.BaseColor = BaseColor;
            blockComponent.ChangeColor(BaseColor, Block.ChangeColorDuration, true);
        }

        var position = block.transform.position;
        ValidateIsAlongColumn(position);
        ValidateIsNotBelowSupportBlock(position);
        ValidateIsOpen(position);

        block.transform.SetParent(transform);
        InsertBlock(blockComponent);
    }

    private void InsertBlock(Block block)
    {
        var insertIndex = Blocks.Count;
        for (var i = 0; i < Blocks.Count; i++)
        {
            var isAbove = Blocks[i].transform.position.y > block.transform.position.y;
            if (isAbove)
            {
                insertIndex = i;
            }
        }

        Blocks.Insert(insertIndex, block);
    }

    private void ValidateIsAlongColumn(Vector3 position)
    {
        var isInColumn = Mathf.Approximately(position.x, transform.position.x) &&
            Mathf.Approximately(position.z, transform.position.z);
        if (!isInColumn)
        {
            throw new ArgumentException(string.Format("{0} is not in the column at {1}. Column local position is {2}",
                position, transform.position, transform.localPosition));
        }
    }

    private void ValidateIsNotBelowSupportBlock(Vector3 position)
    {
        var isBelowLowest = position.y < BlockColumnManager.Instance.SupportBlockHeight + 1;
        if (isBelowLowest)
        {
            throw new ArgumentException(string.Format("{0} is below the support block at {1}", position,
                BlockColumnManager.Instance.SupportBlockHeight));
        }
    }

    private void ValidateIsOpen(Vector3 position)
    {
        var isOpen = true;
        for (var i = 0; i < Blocks.Count; i++)
        {
            if (Mathf.Abs(Blocks[i].transform.position.y - position.y) < 1)
            {
                isOpen = false;
                break;
            }
        }

        if (!isOpen)
        {
            throw new ArgumentException(string.Format("{0} is not open on column at {1}. Column local position is {2}",
                position, transform.position, transform.localPosition));
        }
    }
}