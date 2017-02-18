using System;
using UnityEngine;

/// <summary>
/// Blocks are children GameObjects, in ascending order by y-position.
/// </summary>
[ExecuteInEditMode]
public class BlockColumn : MonoBehaviour
{
    public Vector3 SupportPosition { get { return transform.position + _supportCollider.center; } }

    public Color BaseColor;

    private BoxCollider _supportCollider;


    void Start()
    {
        Initialize();
    }

    public void Initialize()
    {
        _supportCollider = GetComponent<BoxCollider>();
    }

    public void MoveSupportUp()
    {
        _supportCollider.center += Vector3.up;
    }

    public GameObject Remove(Vector3 position)
    {
        GameObject removedBlock = null;
        for (var i = 0; i < transform.childCount; i++)
        {
            var child = transform.GetChild(i);
            if (removedBlock == null && child.position == position)
            {
                removedBlock = child.gameObject;
                removedBlock.GetComponent<Block>().MakeFallAfterSlideBlockDelay();
            }
            else if (removedBlock != null)
            {
                var isRemovedBlockLowest = Mathf.Approximately(removedBlock.transform.position.y, SupportPosition.y + 1);
                if (!isRemovedBlockLowest)
                {
                    child.GetComponent<Block>().MakeFallAfterSlideBlockDelay();
                }
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
        block.GetComponent<Block>().BaseColor = BaseColor;

        var position = block.transform.position;
        ValidateIsAlongColumn(position);
        ValidateIsNotBelowSupportBlock(position);
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

    private void ValidateIsNotBelowSupportBlock(Vector3 position)
    {
        var isBelowLowest = position.y < SupportPosition.y + 1;
        if (isBelowLowest)
        {
            throw new ArgumentException(string.Format("{0} is below the support block at {1}", position,
                SupportPosition.y));
        }
    }

    private void ValidateIsOpen(Vector3 position)
    {
        var isOpen = true;
        for (var i = 0; i < transform.childCount; i++)
        {
            if (Mathf.Abs(transform.GetChild(i).position.y - position.y) < 1)
            {
                isOpen = false;
                break;
            }
        }

        if (!isOpen)
        {
            throw new ArgumentException(string.Format("{0} is not open", position));
        }
    }
}