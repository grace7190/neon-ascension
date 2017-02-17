using UnityEngine;

public class CameraController : MonoBehaviour {

    public bool ShouldGenerateBlocks;
    public Vector3 Velocity = new Vector3(0, 0.04f, 0);
    public float VelocityIncrease = 0.02f;
    public float VelocityIncreasePerSeconds = 60.0f;
    
    void Update () 
    {
        var numberOfIncreases = (int)(Time.timeSinceLevelLoad / VelocityIncreasePerSeconds);
        transform.position += Velocity * (1 + numberOfIncreases) * Time.deltaTime;
    }
}
