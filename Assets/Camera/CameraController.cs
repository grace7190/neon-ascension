using UnityEngine;

public class CameraController : MonoBehaviour {

    public bool ShouldGenerateBlocks;
    public bool CameraScrollEnabled; 
    public float VelocityIncrease = 0.03f;
    public float VelocityIncreasePerSeconds = 60.0f;

    public static CameraController Instance;

    private static readonly Vector3 Velocity = new Vector3(0, 0.10f, 0);

    private GameObject _blueTeamCamera;
    private GameObject _purpleTeamCamera;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        _blueTeamCamera = transform.FindChild("Camera 1").gameObject;
        _purpleTeamCamera = transform.FindChild("Camera 2").gameObject;
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

    public void ShakeCameraForTeam(Team team, float time, float xMove, float yMove)
    {
        var shakeParams = iTween.Hash("x", xMove, "y", yMove, "time", time);
        iTween.ShakePosition(GetCameraForTeam(team), shakeParams);
    }

    public GameObject GetCameraForTeam(Team team)
    {
        switch(team)
        {
            case Team.Blue:
            {
                return _blueTeamCamera;
            }
            case Team.Purple:
            {
                return _purpleTeamCamera;
            }
        }
        return null;
    }
}
