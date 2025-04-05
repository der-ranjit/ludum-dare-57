using UnityEditor;
using UnityEngine;

public class RoomCreatorEditor : EditorWindow
{
    private float planeWidth = 10f; // Width of the plane
    private float planeHeight = 10f; // Height of the plane
    private float wallHeight = 2f; // Height of the walls

    private Material planeMaterial; // Material for the plane
    private Material wallMaterial; // Material for the walls

    [MenuItem("Tools/Room Creator")]
    public static void ShowWindow()
    {
        GetWindow<RoomCreatorEditor>("Room Creator");
    }

    private void OnGUI()
    {
        GUILayout.Label("Room Settings", EditorStyles.boldLabel);

        // Input fields for room dimensions
        planeWidth = EditorGUILayout.FloatField("Plane Width", planeWidth);
        planeHeight = EditorGUILayout.FloatField("Plane Height", planeHeight);
        wallHeight = EditorGUILayout.FloatField("Wall Height", wallHeight);

        // Material fields
        planeMaterial = (Material)EditorGUILayout.ObjectField("Plane Material", planeMaterial, typeof(Material), false);
        wallMaterial = (Material)EditorGUILayout.ObjectField("Wall Material", wallMaterial, typeof(Material), false);

        // Generate Room button
        if (GUILayout.Button("Generate Room"))
        {
            GenerateRoom();
        }
    }

    private void GenerateRoom()
    {
        // Create a parent GameObject for the room
        GameObject room = new GameObject("Room");

        // Create the floor plane
        GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        plane.transform.localScale = new Vector3(planeWidth / 10f, 1f, planeHeight / 10f);
        plane.transform.position = Vector3.zero;
        plane.name = "Floor";
        plane.transform.parent = room.transform;

        if (planeMaterial != null)
        {
            plane.GetComponent<Renderer>().material = planeMaterial;
        }

        // Create walls
        Vector3[] wallPositions = new Vector3[]
        {
            new Vector3(0f, wallHeight / 2f, planeHeight / 2f), // Top wall
            new Vector3(0f, wallHeight / 2f, -planeHeight / 2f), // Bottom wall
            new Vector3(planeWidth / 2f, wallHeight / 2f, 0f), // Right wall
            new Vector3(-planeWidth / 2f, wallHeight / 2f, 0f) // Left wall
        };

        Quaternion[] wallRotations = new Quaternion[]
        {
            Quaternion.Euler(90f, 180f, 0f), // Top wall (rotated to face inward)
            Quaternion.Euler(90f, 0f, 0f), // Bottom wall (rotated to face inward)
            Quaternion.Euler(90f, -90f, 0f), // Right wall (rotated to face inward)
            Quaternion.Euler(90f, 90f, 0f) // Left wall (rotated to face inward)
        };

        Vector3[] wallScales = new Vector3[]
        {
            new Vector3(planeWidth / 10f, 1f, wallHeight / 10f), // Top wall
            new Vector3(planeWidth / 10f, 1f, wallHeight / 10f), // Bottom wall
            new Vector3(planeHeight / 10f, 1f, wallHeight / 10f), // Right wall
            new Vector3(planeHeight / 10f, 1f, wallHeight / 10f) // Left wall
        };

        for (int i = 0; i < 4; i++)
        {
            GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Plane);

            // Scale and position the wall
            wall.transform.localScale = wallScales[i];
            wall.transform.position = wallPositions[i];
            wall.transform.rotation = wallRotations[i];
            wall.name = $"Wall_{i + 1}";
            wall.transform.parent = room.transform;

            if (wallMaterial != null)
            {
                wall.GetComponent<Renderer>().material = wallMaterial;
            }
        }

        // Select the created room in the hierarchy
        Selection.activeGameObject = room;
    }
}