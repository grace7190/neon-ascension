using UnityEngine;

public class CameraController : MonoBehaviour {

    public bool ShouldGenerateBlocks;
    public bool CameraScrollEnabled; 
    public float VelocityIncrease = 0.03f;
    public float VelocityIncreasePerSeconds = 60.0f;

    private static readonly Vector3 Velocity = new Vector3(0, 0.10f, 0);

    void Start()
    { 
    }

    void Update()
    {
        if (BlockColumnManager.Instance.CameraScrollEnabled) { 
            var numberOfIncreases = (int)(Time.timeSinceLevelLoad / VelocityIncreasePerSeconds);
            transform.position += Velocity * (1 + numberOfIncreases) * Time.deltaTime;
        }

        // sorry this is hack -g
        if (Time.time > 29.0 & Time.time < 29.1)
        {
            IncreaseBGMVolume();
        }
    }

    public void IncreaseBGMVolume()
    {
        var audio = GetComponent<AudioSource>();
        audio.volume = 0.7f;
    }
}
