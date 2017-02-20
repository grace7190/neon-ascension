using UnityEngine;

public class CameraController : MonoBehaviour {

    public bool ShouldGenerateBlocks;
    public float VelocityIncrease = 0.02f;
    public float VelocityIncreasePerSeconds = 60.0f;

    private static readonly Vector3 Velocity = new Vector3(0, 0.10f, 0);

    void Update () 
    {
        var numberOfIncreases = (int)(Time.timeSinceLevelLoad / VelocityIncreasePerSeconds);
        transform.position += Velocity * (1 + numberOfIncreases) * Time.deltaTime;
    }
}
