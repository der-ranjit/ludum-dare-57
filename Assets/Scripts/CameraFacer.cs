using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFacer : MonoBehaviour
{

    public float leanBackAngle = 15f; // Angle to lean back when facing the camera

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        // Always face the camera (y axis only)
        Camera mainCamera = Camera.main;
        if (mainCamera != null)
        {
            Vector3 directionToCamera = mainCamera.transform.position - transform.position;
            directionToCamera.y = 0; // Ignore the y-axis to keep the object upright
            Quaternion rotation = Quaternion.LookRotation(directionToCamera);
            transform.rotation = rotation;
            transform.Rotate(-leanBackAngle, 0, 0); // Lean back by the specified angle
        }
    }
}
