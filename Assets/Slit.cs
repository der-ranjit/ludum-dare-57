using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[ExecuteInEditMode]
public class Slit : MonoBehaviour
{

    public int width = 10; // Width of the slit
    public int height = 1; // Height of the slit

    
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
        slit.width = horizontal ? 10 : 1;
        slit.height = horizontal ? 1 : 10;

        return slitInstance;
    }

    // Start is called before the first frame update
    void Start()
    {
        // Create a plane for the slit
        GameObject plane = transform.Find("SlitPlane")?.gameObject;
        plane.transform.localScale = new Vector3(width / 10f, 1f, height / 10f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
