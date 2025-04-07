using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoRotator : MonoBehaviour
{

    public float degreesPerSecond = 120f; // Angle to lean back when facing the camera

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        // y-rotate at degreesPerSecond
        transform.Rotate(0, degreesPerSecond * Time.deltaTime, 0, Space.World);
    }
}
