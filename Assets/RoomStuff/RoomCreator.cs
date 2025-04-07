using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

public static class RoomCreator
{
    private static bool startInBedroom = true; // Flag to indicate if the game starts in the bedroom    
    private static int roomsCreated = startInBedroom ? 0 : -1; // Counter for the number of rooms created
    public static GameObject DeleteAndGenerateRoom(Material wallMaterial, float planeWidth, float planeHeight, Material planeMaterial)
    {
        DeleteCurrentRoom();
        return GenerateRoom(planeWidth, planeHeight);
    }

    public static void ResetRoomsCreated()
    {
        roomsCreated = startInBedroom ? 0 : -1; // Reset the room counter
    }

    public static void DeleteCurrentRoom()
    {
        // Check if a room already exists and delete it
        GameObject existingRoom = GameObject.Find("Room");
        if (existingRoom != null)
        {
            Object.DestroyImmediate(existingRoom);
        }
    }

    public static GameObject GenerateRoom(float planeWidth, float planeHeight)
    {
        Sprite[] allSprites = Resources.LoadAll<Sprite>("Rooms/Forest/Walls");
        foreach (Sprite sprite in allSprites)
        {
            Debug.Log($"Sprite name: {sprite.name}");
        }

        roomsCreated++; // Increment the room counter

        RoomConfig info = RoomConfigs.GetBasicRoomInfo(roomsCreated); // Get room info based on the number of rooms created
        planeWidth = info.width;
        planeHeight = info.height;

        GameObject[] slits; // Array to hold slits
        GameObject[] doors; // Array to hold doors

        // Create a parent GameObject for the room
        GameObject room = new GameObject("Room");

        // Select random wall and floor materials based on the allowed styles
        WallStyle randomWallStyle = info.wallStyles[Random.Range(0, info.wallStyles.Length)];
        Material baserMaterial = Resources.Load<Material>("Rooms/All/Walls/baseMaterial");
        Material wallMaterial = GetWallMaterialForStyle(baserMaterial, randomWallStyle);
        Material floorMaterial = GetFloorMaterialForStyle(baserMaterial, randomWallStyle);

        // Get width and height of wallMaterial texture
        Texture2D wallTexture = wallMaterial.mainTexture as Texture2D;
        float wallHeight = wallTexture.height / 25f; // texture height defines wall height

        // Create the floor plane
        GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        plane.transform.localScale = new Vector3(planeWidth / 10f, 1f, planeHeight / 10f);
        plane.transform.position = Vector3.zero;
        plane.name = "Floor";
        plane.transform.parent = room.transform;

        if (floorMaterial != null)
        {
            plane.GetComponent<Renderer>().material = floorMaterial;
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

        // Create the player spawner.
        GameObject spawner = new GameObject("PlayerSpawner");
        float spawnX = (info.playerSpawnX - 0.5f) * planeWidth;
        float spawnZ = (info.playerSpawnY - 0.5f) * planeHeight;
        spawner.transform.position = new Vector3(spawnX, 0.5f, spawnZ);
        spawner.transform.parent = room.transform;

        GameObject playerPrefab = Resources.Load<GameObject>("Characters/PlayerPrefab");
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
        int slit_count = info.slitCount;
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

        // Create exit door
        Debug.Log($"Door position: {info.doorPos}");
        GameObject door = CreateDoor(info.doorPos, planeWidth, planeHeight, info.wallStyles[0]);
        doors = new GameObject[] { door };
        door.transform.parent = room.transform;

        // Create game objects
        // Create Trees
        for (int i = 0; i < info.treeCount; i++)
        {
            GameObject treePrefab = Resources.Load<GameObject>("Rooms/Forest/Deco/TreePrefab");
            GameObject treeInstance = Object.Instantiate(treePrefab);
            treeInstance.transform.position = GetRandomPosition(2f, planeWidth, planeHeight);
            treeInstance.transform.parent = room.transform;
            treeInstance.name = $"Tree_{i + 1}";
        }
        // Create Stones
        for (int i = 0; i < info.stoneCount; i++)
        {
            GameObject stonePrefab = Resources.Load<GameObject>("Rooms/All/Deco/StonePrefab");
            GameObject stoneInstance = Object.Instantiate(stonePrefab);
            stoneInstance.transform.position = GetRandomPosition(2f, planeWidth, planeHeight);
            stoneInstance.transform.parent = room.transform;
            stoneInstance.name = $"Stone_{i + 1}";
        }
        // Fire
        for (int i = 0; i < info.fireCount; i++)
        {
            GameObject firePrefab = Resources.Load<GameObject>("Lights/CampfirePrefab");
            GameObject fireInstance = Object.Instantiate(firePrefab);
            fireInstance.transform.position = GetRandomPosition(2f, planeWidth, planeHeight);
            fireInstance.transform.parent = room.transform;
            fireInstance.name = $"Fire_{i + 1}";
        }

        // Room finalization of info object, if present
        if (info.finalizeRoom != null)
        {
            info.finalizeRoom(room);
        }

        return room;
    }


    private static GameObject CreateDoor(float wallPos, float planeWidth, float planeHeight, WallStyle wallStyle)
    {
        // wallId is floor of wallPos
        int wallId = Mathf.FloorToInt(wallPos);
        // Load door prefab from Resources folder
        string doorPrefabName = "Rooms/Forest/Doors/ForestDoorArchPrefab";
        switch (wallStyle) {
            case WallStyle.Bedroom:
                doorPrefabName = "Rooms/Bedroom/Doors/BedroomDoorPrefab";
                break;
        }
        GameObject doorPrefab = Resources.Load<GameObject>(doorPrefabName);
        GameObject doorInstance = Object.Instantiate(doorPrefab);
        doorInstance.name = $"Door_{wallId + 1}";

        doorInstance.transform.rotation = Quaternion.Euler(0f, 90f * wallId, 0f); // Reset rotation

        // If only wall side is specified and not exact position, use random factor between -0.25 and +0.25
        float doorPosOnWall = wallPos - wallId;
        if (doorPosOnWall == 0) {
            doorPosOnWall = Random.Range(-0.25f, 0.25f);
        } else {
            doorPosOnWall -= 0.5f; // shift from [0,1] to [-0.5,0.5]
        }

        Debug.Log($"Door position on wall {wallId}: {doorPosOnWall} based on wallPos {wallPos}");

        // Set the position and rotation based on the wall ID
        switch (wallId)
        {
            case 0: // Top wall
                doorInstance.transform.position = new Vector3(doorPosOnWall * planeWidth, 0, planeHeight / 2f - 0.01f);
                break;
            case 1: // Right wall
                doorInstance.transform.position = new Vector3(planeWidth / 2f - 0.01f, 0, doorPosOnWall * planeHeight);
                break;
            case 2: // Bottom wall
                doorInstance.transform.position = new Vector3(doorPosOnWall * planeWidth, 0, -planeHeight / 2f + 0.01f);
                break;
            case 3: // Left wall
                doorInstance.transform.position = new Vector3(-planeWidth / 2f + 0.01f, 0, doorPosOnWall * planeHeight);
                break;
            default:
                Debug.LogError("Invalid wall ID for door creation.");
                return null;
        }

        return doorInstance;
    }

    private static Vector3 GetRandomPosition(float distanceFromWalls, float planeWidth, float planeHeight)
    {
        float x = Random.Range(-planeWidth / 2f + distanceFromWalls, planeWidth / 2f - distanceFromWalls);
        float z = Random.Range(-planeHeight / 2f + distanceFromWalls, planeHeight / 2f - distanceFromWalls);
        return new Vector3(x, 0, z);
    }

    private static Material GetWallMaterialForStyle(Material baseMaterial, WallStyle style)
    {
        return AddRandomTextureToBaseRoomMaterial(baseMaterial, style, "wall");
    }

    private static Material GetFloorMaterialForStyle(Material baseMaterial, WallStyle style)
    {
        return AddRandomTextureToBaseRoomMaterial(baseMaterial, style, "floor");
    }

    private static Material AddRandomTextureToBaseRoomMaterial(Material baseMaterial, WallStyle style, string spriteFilter)
    {
        string path = $"Rooms/{style.ToStringValue()}/Walls";
        // Load all sprites in the specified folder
        Sprite[] sprites = Resources.LoadAll<Sprite>(path);
        // Filter sprites with "wall" in their name
        for (int i = 0; i < sprites.Length; i++)
        {
            Debug.Log($"Sprite name: {sprites[i].name}");
        }
        Sprite[] filteredSprites = System.Array.FindAll(sprites, sprite => sprite.name.ToLower().Contains(spriteFilter.ToLower()));
        if (filteredSprites.Length == 0)
        {
            Debug.LogError($"No sprites found at path: {path}");
            return null;
        }
        // Select a random wall sprite
        Sprite randomSprite = filteredSprites[Random.Range(0, filteredSprites.Length)];
        Debug.Log($"Selected sprite: {randomSprite.name}");
        bool isTransparent = randomSprite.name.ToLower().Contains("transparent");
        // Clone the base material and set the sprite as its base map
        Material material = new Material(baseMaterial);
        material.mainTexture = randomSprite.texture;
        if (isTransparent)
        {
            // enable alpha clipping
            material.SetFloat("_AlphaClip", 1);
            material.SetFloat("_Cutoff", 0.5f);
            material.EnableKeyword("_ALPHATEST_ON");
        }
        return material;
    }
}


