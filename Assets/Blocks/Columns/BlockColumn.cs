using System;
using UnityEngine;

/// <summary>
/// Blocks are children GameObjects, in ascending order by y-position.
/// </summary>
[ExecuteInEditMode]
public class BlockColumn : MonoBehaviour
{
    public GameObject Remove(Vector3 position)
    {
        GameObject removedBlock = null;
        for (var i = 0; i < transform.childCount; i++)
        {
            var child = transform.GetChild(i);
            if (removedBlock == null && child.position == position)
            {
                removedBlock = child.gameObject;
                removedBlock.GetComponent<Block>().MakeFall();
            }
            else if (removedBlock != null)
            {
                child.GetComponent<Block>().MakeFall();
            }
        }

        if (removedBlock == null)
        {
            throw new ArgumentException(string.Format("No block at {0} in column at {1}", position,
                transform.position));
        }

        removedBlock.transform.SetParent(null);

        return removedBlock;
    }

    public void Add(GameObject block)
    {
        var position = block.transform.position;
        ValidateIsAlongColumn(position);
        ValidateIsNotBelowLowestBlock(position);
        ValidateIsOpen(position);

        block.transform.SetParent(transform);
        block.transform.SetSiblingIndex(GetSiblingIndex(position));
    }
    
    private int GetSiblingIndex(Vector3 position)
    {
        for (var i = 0; i < transform.childCount; i++)
        {
            var isAbove = transform.GetChild(i).transform.position.y > position.y;
            if (isAbove)
            {
                return i;
            }
        }

        return transform.childCount - 1;
    }

    private void ValidateIsAlongColumn(Vector3 position)
    {
        var isInColumn = Mathf.Approximately(position.x, transform.position.x) && 
            Mathf.Approximately(position.z, transform.position.z);
        if (!isInColumn)
        {
            throw new ArgumentException(string.Format("{0} is not in the column at {1}", position, transform.position));
        }
    }

    private void ValidateIsNotBelowLowestBlock(Vector3 position)
    {
        if (transform.childCount > 0)
        {
            var lowestBlockPosition = transform.GetChild(0).transform.position;
            var isBelowLowest = position.y < lowestBlockPosition.y;
            if (isBelowLowest)
            {
                throw new ArgumentException(string.Format("{0} is below the lowest block at {1}", position,
                    lowestBlockPosition.y));
            }
        }
    }

    private void ValidateIsOpen(Vector3 position)
    {
        var isOpen = transform.childCount == 0;
        for (var i = 0; i < transform.childCount; i++)
        {
            var currBlockIsBelow = transform.GetChild(i).position.y <= position.y - 1;
            var currBlockIsLast = i == transform.childCount - 1;
            var aboveCurrBlockIsOpen = currBlockIsLast || transform.GetChild(i + 1).position.y >= position.y + 1;
            if (currBlockIsBelow && aboveCurrBlockIsOpen)
            {
                isOpen = true;
            }
        }

        if (!isOpen)
        {
            throw new ArgumentException(string.Format("{0} is not open", position));
        }
    }
}