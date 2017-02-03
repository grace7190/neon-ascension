using UnityEngine;

public class PlatformButtonBehavior : MonoBehaviour {

    public GameObject platform;

    void Start () {
    }
	
	void FixedUpdate () {
        //// y is between 5.74 and 8
        platform.transform.Translate(0, -0.05f, 0);
        if (platform.transform.position.y < 5.74f)
        {
            platform.transform.position = new Vector3(platform.transform.position.x, 5.74f, platform.transform.position.z); ;
        }
    }

    void OnTriggerStay(Collider other)
    {   
        platform.transform.Translate(0, 0.10f, 0);
        if (platform.transform.position.y > 8.0f)
        {
            platform.transform.position = new Vector3(platform.transform.position.x, 8.0f, platform.transform.position.z); ;
        }
    }

}
