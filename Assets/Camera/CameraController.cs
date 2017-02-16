using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    public bool ShouldGenerateBlocks = false;
    public BlockGenerator Generator;
    public Vector3 Velocity = new Vector3(0, 0.04f, 0);
    public float VelocityIncrease = 0.02f;
    public float VelocityIncreasePerSeconds = 60.0f;

    private const float BlockHeight = 1;
    private float _distanceMovedSinceGenerating;

    void Start () {}

    void Update () 
    {
        int numberOfIncreases = (int)(Time.timeSinceLevelLoad / VelocityIncreasePerSeconds);
        GetComponent<Rigidbody>().velocity = Velocity * (1 + numberOfIncreases);
    }

    void FixedUpdate() 
    {   
        _distanceMovedSinceGenerating += GetComponent<Rigidbody>().velocity.y * Time.fixedDeltaTime;
        int rowsToGenerate = (int)(_distanceMovedSinceGenerating / BlockHeight);

        if (ShouldGenerateBlocks && rowsToGenerate > 0) 
        {
            Generator.AddRow();
            _distanceMovedSinceGenerating -= rowsToGenerate * BlockHeight;
        }
    }
}
