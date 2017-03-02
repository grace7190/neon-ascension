using UnityEngine;

public class CollisionMananger : MonoBehaviour {

    void Start ()
    {
        Physics.IgnoreLayerCollision(Layers.Player, Layers.BlockColumnSupport);
        Physics.IgnoreLayerCollision(Layers.IgnoreColumnSupport, Layers.BlockColumnSupport);
    }
}
