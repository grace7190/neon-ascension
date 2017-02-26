using UnityEngine;

public class CollisionMananger : MonoBehaviour {

	void Start ()
	{
		Physics.IgnoreLayerCollision(Layers.Player, Layers.BlockColumnSupport);
	}
}
