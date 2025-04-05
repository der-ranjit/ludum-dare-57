using UnityEditor;
using UnityEngine;

public class RoomCreatorEditor : EditorWindow
{
    private float planeWidth = 10f; // Width of the plane
    private float planeHeight = 10f; // Height of the plane
    private float wallHeight = 2f; // Height of the walls

    private Material planeMaterial; // Material for the plane
    private Material wallMaterial; // Material for the walls

    private GameObject[] slits; // Array to hold slits

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

        // Load forestMaterial material from Resources folder
        Material wallMaterial = Resources.Load<Material>("RoomStuff/forest/forestMaterial");
        // Get width and height of wallMaterial texture
        Texture2D wallTexture = wallMaterial.mainTexture as Texture2D;
        wallHeight = wallTexture.height / 25f; // texture height defines wall height


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
        new Vector3(planeWidth / 2f, wallHeight / 2f, 0f), // Right wall
        new Vector3(0f, wallHeight / 2f, -planeHeight / 2f), // Bottom wall
        new Vector3(-planeWidth / 2f, wallHeight / 2f, 0f) // Left wall
        };

        Quaternion[] wallRotations = new Quaternion[]
        {
        Quaternion.Euler(90f, 180f, 0f), // Top wall (rotated to face inward)
        Quaternion.Euler(90f, -90f, 0f), // Right wall (rotated to face inward)
        Quaternion.Euler(90f, 0f, 0f), // Bottom wall (rotated to face inward)
        Quaternion.Euler(90f, 90f, 0f) // Left wall (rotated to face inward)
        };

        Vector3[] wallScales = new Vector3[]
        {
        new Vector3(planeWidth / 10f, 1f, wallHeight / 10f), // Top wall
        new Vector3(planeHeight / 10f, 1f, wallHeight / 10f), // Right wall
        new Vector3(planeWidth / 10f, 1f, wallHeight / 10f), // Bottom wall
        new Vector3(planeHeight / 10f, 1f, wallHeight / 10f) // Left wall
        };

        float textureOffsetX = 0f; // Texture offset for the wall material to match on edges
        for (int i = 0; i < 4; i++)
        {
            GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Plane);

            // Scale and position the wall
            wall.transform.localScale = wallScales[i];
            wall.transform.position = wallPositions[i];
            wall.transform.rotation = wallRotations[i];
            wall.name = $"Wall_{i + 1}";
            wall.transform.parent = room.transform;


            // Adjust tiling.x value of wallMaterial
            if (wallMaterial != null)
            {
                // Clone the wall material
                Material tempWallMaterial = new Material(wallMaterial);
                float expectedWidth = wallTexture.width / 25f;
                float texScale = 10 * wallScales[i].x / expectedWidth;
                tempWallMaterial.mainTextureScale = new Vector2(texScale, 1);
                // Adjust material texture offset
                tempWallMaterial.mainTextureOffset = new Vector2(textureOffsetX, 0);
                float offsetX = texScale % 1f;
                // Adjust the offset for the next wall
                textureOffsetX += offsetX;
                textureOffsetX %= 1f; // Keep it within 0-1 range
                Debug.Log($"Wall {i + 1} Texture Offset: {textureOffsetX} due to added offset {offsetX} based on scale {texScale}");
                wall.GetComponent<Renderer>().material = tempWallMaterial;
            }
        }

        // Create the player spawner
        GameObject spawner = new GameObject("PlayerSpawner");
        spawner.transform.position = new Vector3(0f, 1f, 0f); // Place it slightly above the floor
        spawner.transform.parent = room.transform;

        // Add a visual indicator for the spawner (e.g., a sphere)
        GameObject spawnerIndicator = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        spawnerIndicator.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f); // Make the sphere smaller
        spawnerIndicator.transform.position = spawner.transform.position;
        spawnerIndicator.name = "SpawnerIndicator";
        spawnerIndicator.transform.parent = spawner.transform;

        // Disable the collider on the spawner indicator
        DestroyImmediate(spawnerIndicator.GetComponent<Collider>());

        GameObject playerPrefab = Resources.Load<GameObject>("TessaPrefab");
        // Instantiate the player prefab at the spawner's position
        if (playerPrefab != null)
        {
            GameObject player = (GameObject)PrefabUtility.InstantiatePrefab(playerPrefab);
            player.transform.position = spawner.transform.position;
            player.name = "Player";
            player.transform.parent = room.transform;

            // Remove the spawner and its indicator after spawning the player
            DestroyImmediate(spawner);
        }

        // Create slits
        GameObject slit1 = Slit.CreateSlit(room, new Vector2(2, 1), true);
        GameObject slit2 = Slit.CreateSlit(room, new Vector2(-1, 0), false);
        this.slits = new GameObject[] { slit1, slit2 };

        // Select the created room in the hierarchy
        Selection.activeGameObject = room;
    }
}