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
        // Create a plane for the slit
        GameObject plane = transform.Find("SlitPlane")?.gameObject;
        plane.transform.localScale = new Vector3(width / 10f, 1f, height / 10f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
