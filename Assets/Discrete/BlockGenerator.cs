using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class BlockGenerator : MonoBehaviour
{
    public GameObject BlockPrefab;
    public GameObject HolePrefab;
    public Vector3 Dimensions;
    public List<Vector3> Holes;
    public Color BaseColor;

	void Start ()
	{
	    while (transform.childCount > 0)
	    {
	        var child = transform.GetChild(0);
            DestroyImmediate(child.gameObject);
        }

	    for (var x = 0; x < Dimensions.x; x++)
	    {
            for (var y = 0; y < Dimensions.y; y++)
            {
                for (var z = 0; z < Dimensions.z; z++)
                {
                    var v = new Vector3(x, y, z);
                    if (Holes.Contains(v))
                    {
                        var hole = Instantiate(HolePrefab);
                        hole.transform.SetParent(transform);
                        hole.transform.localPosition = v;
                    }
                    else
                    {
                        var block = Instantiate(BlockPrefab);
                        block.transform.SetParent(transform);
                        block.transform.localPosition = v;
                        var material = new Material(block.GetComponent<MeshRenderer>().sharedMaterial);
                        var color = BaseColor * (Random.value / 4f + 0.5f);
                        material.color = new Color(color.r, color.g, color.b, BaseColor.a);
                        block.GetComponent<MeshRenderer>().sharedMaterial = material;
                        
                    }
                }
            }
        }
	}
}
