using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillboardDuplicator : MonoBehaviour
{

    public int numberOfBillboards = 1; // Number of duplicates. 1 means we'll have two in total (original + 1 extra), rotated by 90°

    // Start is called before the first frame update
    void Start()
    {
        // Take child SpriteRenderer as root to duplicate
        SpriteRenderer spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError("No SpriteRenderer found in children. Please attach a SpriteRenderer to the child object.");
            return;
        }
        if (spriteRenderer.gameObject == gameObject) {
            Debug.LogError("The SpriteRenderer is on the same GameObject as the BillboardDuplicator. Please attach it to a child object.");
            return;
        }
        float angleStep = 180f / (numberOfBillboards + 1); // Calculate the angle step for each duplicate
        // Create the duplicates
        for (int i = 1; i <= numberOfBillboards; i++)
        {
            GameObject duplicate = Instantiate(spriteRenderer.gameObject, transform);
            duplicate.name = spriteRenderer.gameObject.name + "_Duplicate_" + i; // Rename the duplicate for clarity
            duplicate.transform.position = spriteRenderer.transform.position;
            duplicate.transform.localRotation = Quaternion.Euler(0, angleStep * i, 0);
            Debug.Log("Angle is " + angleStep * i + " vs original " + spriteRenderer.transform.rotation.eulerAngles.y);
            // Add to parent
            duplicate.transform.parent = transform; // Set the parent to the original object
        }
        Destroy(this); // Destroy this script to avoid duplicating it on each instance
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
