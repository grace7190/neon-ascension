using UnityEngine;

public class AutomovePlatformBehavior : MonoBehaviour {
    
    private bool goingUp;
    // Use this for initialization
    void Start()
    {
        goingUp = true;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //// y is between 11.91 and 14.94
        if (goingUp)
        {
            transform.Translate(0, 0.02f, 0);
            if (transform.position.y > 16.0f)
            {
                goingUp = false;
            }
        }
        else
        {
            transform.Translate(0, -0.02f, 0);
            if (transform.position.y < 11.0f)
            {
                goingUp = true;
            }
        }
    }
}
