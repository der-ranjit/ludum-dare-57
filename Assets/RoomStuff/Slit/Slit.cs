using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


[ExecuteInEditMode]
public class Slit : MonoBehaviour
{

    private const float defaultLength = 1.4f;
    private const float defaultWidth = 0.14f;

    public float width = defaultLength; // Width of the slit
    public float height = defaultWidth; // Height of the slit

    
    // Factory method to create a Slit
    public static GameObject CreateSlit(GameObject parent, Vector2 pos, bool horizontal)
    {
        // Slit prefab exists, instantiate it
        GameObject slitPrefab = Resources.Load<GameObject>("RoomStuff/Slit/Slit"); // Load the prefab from Resources folder
        GameObject slitInstance = Instantiate(slitPrefab, parent.transform);
        slitInstance.transform.localPosition = new Vector3(pos.x, 0.01f, pos.y); // Set the position of the slit
        // Get the Slit component
        Slit slit = slitInstance.GetComponent<Slit>();
        // Set the slit dimensions
        slit.width = horizontal ? defaultLength : defaultWidth;
        slit.height = horizontal ? defaultWidth : defaultLength;

        return slitInstance;
    }

    // Start is called before the first frame update
    void Start()
    {
        // Scale the object accordingly to given values
        float scaleFactor = Mathf.Max(width, height) / 10f;
        // GameObject plane = transform.Find("SlitPlane")?.gameObject;
        if (width > height) {
            transform.localRotation = Quaternion.Euler(0, 0, 0); // Default rotation for horizontal slit
            transform.localScale = new Vector3(width / 10f, scaleFactor, height / 1f); // Scale the plane based on width and height
        }
        else
        {
            transform.localRotation = Quaternion.Euler(0, 90, 0); // Rotate the plane for vertical slit
            transform.localScale = new Vector3(height / 10f, scaleFactor, width / 1f);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerStay(Collider other)
    {
        // Check if other has "SlitVictim" script attached
        SlitVictim slitVictim = other.GetComponent<SlitVictim>();
        if (slitVictim != null)
        {
            // Check if y angle of victim is aligned with the slit
            Vector3 victimAngle = other.transform.eulerAngles;
            Vector3 slitAngle = transform.eulerAngles;
            float angleDiff = Mathf.Abs(victimAngle.y - slitAngle.y);
            if (angleDiff < 10 || Math.Abs(angleDiff - 180) < 10) {
                // Trigger the slit victim if aligned
                slitVictim.Trigger(); // Call the Trigger method on the SlitVictim script
                Debug.Log($"Victim Angle: {victimAngle.y}, Slit Angle: {slitAngle.y}, Angle Difference: {angleDiff}");
            }
            else
            {
                Debug.Log("SlitVictim not aligned with the slit!");
            }
        }
    }

}
