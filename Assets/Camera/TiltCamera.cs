using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TiltCamera : MonoBehaviour
{

    public GameObject player;
    public GameObject blockColumnManager;
    public float maxAngleChange;            // Max degree of angle to change from initial angle
    public float maxMovement;               // Max offset to move the camera for tilting
    public int maxFrameUpdate;              // Number of frames used to update tilting to target.

    private BoxCollider supportBoxCollider;
    private float maxDiff = 15.0f;          // maxDiff is set to the wall height
    private float anglePerDiff;
    private float movePerDiff;
    private float initAngle;
    private float initDiff;
    private float preOffset;                // Previous offset
    private float anglePerFrame;            // How many degrees to tilt per frame
    private float movePerFrame;             // The distance to move per frame for tilting

    // Use this for initialization
    void Start()
    {
        supportBoxCollider = blockColumnManager.GetComponent<BoxCollider>();
        initDiff = Mathf.Floor(player.transform.position.y - supportBoxCollider.center.y);
        anglePerDiff = maxAngleChange / maxDiff;
        movePerDiff = maxMovement / maxDiff;
        initAngle = transform.rotation.eulerAngles.x;
        preOffset = 0;
        anglePerFrame = 0;
        movePerFrame = 0;
        StartCoroutine(CameraSpinCoroutine());
    }

    // Update is called once per frame
    void Update()
    {

        // Tilt camera only when player has at least 1 life.
        if (player != null && player.activeInHierarchy)
        {
            float curDiff = Mathf.Floor(player.transform.position.y - supportBoxCollider.center.y);
            float actDiff = Mathf.Max(0, curDiff - initDiff);
            float curAngle = initAngle + anglePerDiff * actDiff;
            float curOffset = movePerDiff * actDiff; // How much the camera should have been offset
            float offsetDiff = curOffset - preOffset; // The Amount we need to move

            anglePerFrame = Mathf.DeltaAngle(transform.rotation.eulerAngles.x, curAngle) / maxFrameUpdate;
            movePerFrame = offsetDiff / maxFrameUpdate; // The ammount we move per frame

            transform.rotation = Quaternion.Euler(
                transform.rotation.eulerAngles.x + anglePerFrame,
                transform.rotation.eulerAngles.y,
                transform.rotation.eulerAngles.z
            );

            // Don't let the Y movement go over MaxDiff or below 0
            if (preOffset + movePerFrame > maxDiff)
            {
                movePerFrame = maxDiff - preOffset;
            }
            else if (preOffset + movePerFrame < 0)
            {
                movePerFrame = 0 - preOffset;
            }

            preOffset += movePerFrame;
            transform.position = new Vector3(
                transform.position.x,
                transform.position.y + movePerFrame,
                transform.position.z
            );
        }
    }

    IEnumerator CameraSpinCoroutine()
    {
        var degree = 0.0f;
        while (degree < 360.0f)
        {
            transform.RotateAround(Vector3.zero, Vector3.up, 2.5f);
            degree += 2.5f;
            yield return new WaitForEndOfFrame();
        }

        GameController.Instance.EnableGame();
    }
}
