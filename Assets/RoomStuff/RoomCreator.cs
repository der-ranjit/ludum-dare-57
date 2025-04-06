using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class RoomCreator 
{
    public static GameObject DeleteAndGenerateRoom(Material wallMaterial, float planeWidth, float planeHeight, Material planeMaterial)
    {
        // Check if a room already exists and delete it
        GameObject existingRoom = GameObject.Find("Room");
        if (existingRoom != null)
        {
            Object.DestroyImmediate(existingRoom);
        }

        // Generate a new room
        return GenerateRoom(wallMaterial, planeWidth, planeHeight, planeMaterial);
    }

    public static GameObject GenerateRoom(Material wallMaterial, float planeWidth, float planeHeight, Material planeMaterial)
    {
        GameObject[] slits; // Array to hold slits
        GameObject[] doors; // Array to hold doors

        // Create a parent GameObject for the room
        GameObject room = new GameObject("Room");

        if (wallMaterial == null)
        {
            // Load forestMaterial material from Resources folder
            wallMaterial = Resources.Load<Material>("forestMaterial");
        }
        if (planeMaterial == null)
        {
            // Load forestMaterial material from Resources folder
            planeMaterial = Resources.Load<Material>("forestFloorMaterial");
        }
        // Get width and height of wallMaterial texture
        Texture2D wallTexture = wallMaterial.mainTexture as Texture2D;
        float wallHeight = wallTexture.height / 25f; // texture height defines wall height


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
            // Two-sided shadow casting
            wall.GetComponent<Renderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.TwoSided;


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
        spawner.transform.position = new Vector3(0f, 0.5f, 0f);
        spawner.transform.parent = room.transform;

        GameObject playerPrefab = Resources.Load<GameObject>("PlayerPrefab");
        // Instantiate the player prefab at the spawner's position
        if (playerPrefab != null)
        {
            GameObject player = Object.Instantiate(playerPrefab, spawner.transform);
            player.name = "Player";
            player.transform.parent = room.transform;

            // Remove the spawner and its indicator after spawning the player
            Object.DestroyImmediate(spawner);
        }

        // Create slits
        int slit_count = Random.Range(1, 10);
        slits = new GameObject[slit_count];
        for (int i = 0; i < slit_count; i++)
        {
            GameObject slit = Slit.CreateSlit(
                room,
                new Vector2(Random.Range(-planeWidth / 2f, planeWidth / 2f),
                Random.Range(-planeHeight / 2f, planeHeight / 2f)),
                Random.Range(0, 2) == 0
            );
            slit.transform.parent = room.transform;
            // Store in slits array
            slits[i] = slit;
            slit.name = $"Slit_{i + 1}";
        }

        // Create exit doors
        int doorId = Random.Range(0, 4);
        GameObject door1 = CreateDoor(doorId, planeWidth, planeHeight);
        doors = new GameObject[] { door1 };
        door1.transform.parent = room.transform;

        // Create game objects
        // Create Trees
        for (int i = 0; i < 3; i++)
        {
            GameObject treePrefab = Resources.Load<GameObject>("TreePrefab");
            GameObject treeInstance = Object.Instantiate(treePrefab);
            treeInstance.transform.position = GetRandomPosition(2f, planeWidth, planeHeight);
            treeInstance.transform.parent = room.transform;
            treeInstance.name = $"Tree_{i + 1}";
        }
        // Create Stones
        for (int i = 0; i < 3; i++)
        {
            GameObject stonePrefab = Resources.Load<GameObject>("StonePrefab");
            GameObject stoneInstance = Object.Instantiate(stonePrefab);
            stoneInstance.transform.position = GetRandomPosition(2f, planeWidth, planeHeight);
            stoneInstance.transform.parent = room.transform;
            stoneInstance.name = $"Stone_{i + 1}";
        }
        // Fire
        GameObject firePrefab = Resources.Load<GameObject>("campfirePrefab");
        GameObject fireInstance = Object.Instantiate(firePrefab);
        fireInstance.transform.position = GetRandomPosition(2f, planeWidth, planeHeight);
        fireInstance.transform.parent = room.transform;
        fireInstance.name = $"Fire_{1}";


        return room;
    }

    private static GameObject CreateDoor(int wallId, float planeWidth, float planeHeight)
    {
        // Load door prefab from Resources folder
        GameObject doorPrefab = Resources.Load<GameObject>("DoorPrefab");
        GameObject doorInstance = Object.Instantiate(doorPrefab);
        doorInstance.name = $"Door_{wallId + 1}";

        doorInstance.transform.rotation = Quaternion.Euler(0f, 90f * wallId, 0f); // Reset rotation

        // Random factor between -0.25 and +0.25
        float randomFactor = Random.Range(-0.25f, 0.25f);

        // Set the position and rotation based on the wall ID
        switch (wallId)
        {
            case 0: // Top wall
                doorInstance.transform.position = new Vector3(randomFactor * planeWidth, 0, planeHeight / 2f - 0.01f);
                break;
            case 1: // Right wall
                doorInstance.transform.position = new Vector3(planeWidth / 2f - 0.01f, 0, randomFactor * planeHeight);
                break;
            case 2: // Bottom wall
                doorInstance.transform.position = new Vector3(randomFactor * planeWidth, 0, -planeHeight / 2f + 0.01f);
                break;
            case 3: // Left wall
                doorInstance.transform.position = new Vector3(-planeWidth / 2f + 0.01f, 0, randomFactor * planeHeight);
                break;
            default:
                Debug.LogError("Invalid wall ID for door creation.");
                return null;
        }

        return doorInstance;
    }

    private static Vector3 GetRandomPosition(float distanceFromWalls, float planeWidth, float planeHeight) {
        float x = Random.Range(-planeWidth / 2f + distanceFromWalls, planeWidth / 2f - distanceFromWalls);
        float z = Random.Range(-planeHeight / 2f + distanceFromWalls, planeHeight / 2f - distanceFromWalls);
        return new Vector3(x, 0, z);
    }
}