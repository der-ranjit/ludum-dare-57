using System.Collections.Generic;
using UnityEngine;

public static class RoomCreator
{
    private static bool startInBedroom = true; // Flag to indicate if the game starts in the bedroom    
    private static int roomsCreated = startInBedroom ? -1 : -2; // Counter for the number of rooms created
    private static bool stayingInRoom = false;
    public static GameObject DeleteAndGenerateRoom(Material wallMaterial, float planeWidth, float planeHeight, Material planeMaterial, GameObject playerPrefab)
    {
        DeleteCurrentRoom();
        return GenerateRoom(planeWidth, planeHeight, playerPrefab);
    }

    // System time now. Used for random seed.
    private static int startTime = System.DateTime.Now.Millisecond;

    public static void StayInRoom()
    {
        if (!stayingInRoom)
        {
            stayingInRoom = true;
            roomsCreated--;
        }
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

    // Based on created rooms, dims the main camera skylight color from its current to black, and changes the global directional light intensity, based on mins of both values.
    // current min should be reached after 8 rooms
    public static void DimTheLights()
    {
        // Define the maximum number of rooms after which the lights are fully dimmed
        int maxRooms = 16;

        // Customizable minimum values
        float minLightIntensity = 0.15f; // Minimum directional light intensity
        Color startColor = new Color(0.27f, 0.68f, 0.69f); // Starting color (#46AEB0)
        Color minAmbientColor = Color.black; // Minimum ambient light color (fully black)
        Color minBackgroundColor = Color.black; // Minimum background color for the main camera

        // Calculate the dimming factor based on the number of rooms created
        float dimFactor = Mathf.Clamp01((float)roomsCreated / maxRooms);

        // Dim the ambient light color (from startColor to minAmbientColor)
        RenderSettings.ambientLight = Color.Lerp(startColor, minAmbientColor, dimFactor);

        // Find the global directional light in the scene
        Light directionalLight = GameObject.FindObjectOfType<Light>();
        if (directionalLight != null && directionalLight.type == LightType.Directional)
        {
            // Dim the directional light intensity (from 1 to minLightIntensity)
            directionalLight.intensity = Mathf.Lerp(1f, minLightIntensity, dimFactor);
        }

        // Find the main camera and adjust its background color
        Camera mainCamera = Camera.main;
        if (mainCamera != null && mainCamera.clearFlags == CameraClearFlags.SolidColor)
        {
            // Dim the background color (from startColor to minBackgroundColor)
            mainCamera.backgroundColor = Color.Lerp(startColor, minBackgroundColor, dimFactor);
        }
    }

    public static GameObject GenerateRoom(float planeWidth, float planeHeight, GameObject playerPrefab)
    {
        PhysicMaterial frictionLessMaterial = Resources.Load<PhysicMaterial>("FrictionLess");

        Sprite[] allSprites = Resources.LoadAll<Sprite>("Rooms/Forest/Walls");
        foreach (Sprite sprite in allSprites)
        {
            Debug.Log($"Sprite name: {sprite.name}");
        }
        DimTheLights();
        roomsCreated++; // Increment the room counter
        stayingInRoom = false; // Reset the staying in room flag
        Random.InitState(startTime + roomsCreated); // Initialize the random seed with the room number

        Debug.Log("Generating room number " + roomsCreated);
        RoomConfig info = RoomConfigs.GetBasicRoomInfo(roomsCreated); // Get room info based on the number of rooms created
        planeWidth = info.width;
        planeHeight = info.height;

        GameObject[] slits; // Array to hold slits
        GameObject[] doors; // Array to hold doors

        // Create a parent GameObject for the room
        GameObject room = new GameObject("Room");
        // It does what it sounds like
        GameObject deathCube = new GameObject("DeathCube");
        deathCube.transform.parent = room.transform;
        // offset cube planes from main room
        float deathCubeOffset = 1.4f;

        // Select random wall and floor materials based on the allowed styles
        RoomStyle wallStyle = info.wallStyles[Random.Range(0, info.wallStyles.Length)];
        Material baserMaterial = Resources.Load<Material>("Rooms/All/Walls/baseMaterial");
        Material wallMaterial = GetWallMaterialForStyle(baserMaterial, wallStyle);
        Material floorMaterial = GetFloorMaterialForStyle(baserMaterial, wallStyle);

        // Get width and height of wallMaterial texture
        Texture2D wallTexture = wallMaterial.mainTexture as Texture2D;
        float wallHeight = wallTexture.height / 25f; // texture height defines wall height

        // Create the floor plane
        GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        plane.transform.localScale = new Vector3(planeWidth / 10f, 1f, planeHeight / 10f);
        plane.transform.position = Vector3.zero;
        plane.name = "Floor";
        plane.transform.parent = room.transform;
        plane.GetComponent<MeshCollider>().material = frictionLessMaterial;
        GameObject deathPlane = DeathTriggerPlane.CreateDeathPlane(plane);
        Vector3 deathPlanePosition = deathPlane.transform.position;
        deathPlanePosition.y = -deathCubeOffset;
        deathPlane.transform.position = deathPlanePosition;
        deathPlane.transform.localScale *= 5f;
        deathPlane.transform.parent = deathCube.transform;

        if (floorMaterial != null)
        {
            plane.GetComponent<Renderer>().material = floorMaterial;
            // If texture scale is <1, we know we should repeat it, haha
            if (floorMaterial.mainTextureScale.x < 0)
            {
                float scaleX = planeWidth / (floorMaterial.mainTexture.width / 25f);
                float scaleY = planeHeight / (floorMaterial.mainTexture.height / 25f);
                Debug.Log($"Floor texture scale: {floorMaterial.mainTextureScale.x} -> {scaleX}, {scaleY}");
                floorMaterial.mainTextureScale = new Vector2(scaleX, scaleY);
            }
        }

        float wallEpsilon = 0.1f; // Small offset to prevent corner gaps
        // Create ceiling, if and only if randomWallStyle has "ceiling" Sprite
        string path = $"Rooms/{wallStyle.ToStringValue()}/Walls";
        // Check if 'ceiling.png' exists in path
        Sprite ceilingSprite = Resources.Load<Sprite>($"{path}/ceiling");
        if (ceilingSprite != null)
        {
            GameObject ceiling = GameObject.CreatePrimitive(PrimitiveType.Plane);
            ceiling.transform.localScale = new Vector3(planeWidth / 10f, 1f, planeHeight / 10f);
            ceiling.transform.position = new Vector3(0f, wallHeight - wallEpsilon, 0f);
            ceiling.transform.rotation = Quaternion.Euler(180f, 0f, 0f); // Rotate to face downward
            ceiling.name = "Ceiling";
            ceiling.transform.parent = room.transform;
            ceiling.GetComponent<MeshCollider>().material = frictionLessMaterial;
            // Clone floor material
            ceiling.GetComponent<Renderer>().material = new Material(floorMaterial);
            ceiling.GetComponent<Renderer>().material.mainTexture = ceilingSprite.texture;
            ceiling.GetComponent<Renderer>().material.mainTextureScale = new Vector2(planeWidth / (ceilingSprite.texture.width / 25f), planeHeight / (ceilingSprite.texture.height / 25f));
            // Two-sided shadow casting
            ceiling.GetComponent<Renderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.TwoSided;

            GameObject deathCeiling = DeathTriggerPlane.CreateDeathPlane(ceiling);
            deathCeiling.transform.parent = deathCube.transform;
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
        new Vector3((planeWidth + wallEpsilon) / 10f, 1f, wallHeight / 10f), // Top wall
        new Vector3((planeHeight + wallEpsilon) / 10f, 1f, wallHeight / 10f), // Right wall
        new Vector3((planeWidth + wallEpsilon) / 10f, 1f, wallHeight / 10f), // Bottom wall
        new Vector3((planeHeight + wallEpsilon) / 10f, 1f, wallHeight / 10f) // Left wall
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
            wall.GetComponent<MeshCollider>().material = frictionLessMaterial;

            // Two-sided shadow casting
            wall.GetComponent<Renderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.TwoSided;

            GameObject deathWall = DeathTriggerPlane.CreateDeathPlane(wall);
            deathWall.transform.parent = deathCube.transform;

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


        deathCube.transform.localScale = Vector3.one * deathCubeOffset;

        // Create the player spawner.
        GameObject spawner = new GameObject("PlayerSpawner");
        float spawnX = (info.playerSpawnX - 0.5f) * planeWidth;
        float spawnZ = (info.playerSpawnY - 0.5f) * planeHeight;
        spawner.transform.position = new Vector3(spawnX, 0.5f, spawnZ);
        spawner.transform.parent = room.transform;

        // Instantiate the player prefab at the spawner's position
        if (playerPrefab == null)
        {
            playerPrefab = Resources.Load<GameObject>("Characters/PlayerPrefab");
        }
        if (playerPrefab != null)
        {
            GameObject player = Object.Instantiate(playerPrefab, spawner.transform);
            player.name = "Player";
            player.transform.parent = room.transform;
            // Look towards origin (but only apply to y axis)
            Vector3 lookAt = new Vector3(0, player.transform.position.y, 0);
            player.transform.LookAt(lookAt);
            // Then round y angle to nearest 90° step
            float yAngle = player.transform.eulerAngles.y;
            yAngle = Mathf.Round(yAngle / 90f) * 90f;
            player.transform.eulerAngles = new Vector3(0, yAngle, 0);

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
                Random.Range(0, 2) == 0,
                wallStyle.ToStringValue()
            );
            slit.transform.parent = room.transform;
            // Store in slits array
            slits[i] = slit;
            slit.name = $"Slit_{i + 1}";
        }

        // Create exit door
        Debug.Log($"Door position: {info.doorPos}");
        GameObject door = CreateDoor(info.doorPos, planeWidth, planeHeight, info.wallStyles[0], wallStyle.ToStringValue());
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

        // Create suitable deco elements
        if (info.decoCount > 0)
        {
            // Always add 'All' to deco styles
            RoomStyle[] allDecoStyles = new RoomStyle[info.decoStyles.Length + 1];
            allDecoStyles[0] = RoomStyle.All;
            for (int i = 0; i < info.decoStyles.Length; i++)
            {
                allDecoStyles[i + 1] = info.decoStyles[i];
            }
            CreateThemedDecoElements(allDecoStyles, info.decoCount, room, planeWidth, planeHeight);
        }

        // Spawn enemies with position filter
        System.Func<Vector3, bool> enemyPositionFilter = (Vector3 pos) =>
        {
            // Avoid spawning too close to player spawn position
            float playerSpawnX = (info.playerSpawnX - 0.5f) * planeWidth;
            float playerSpawnZ = (info.playerSpawnY - 0.5f) * planeHeight;
            Vector3 playerSpawnPos = new Vector3(playerSpawnX, 0, playerSpawnZ);
            float minPlayerDistance = 5.0f;

            if (Vector3.Distance(new Vector3(pos.x, 0, pos.z), playerSpawnPos) < minPlayerDistance)
            {
                return false;
            }

            // Avoid spawning too close to other room objects
            foreach (Transform child in room.transform)
            {
                // Skip objects that are enemies themselves
                if (child.name.StartsWith("Enemy_"))
                    continue;

                float minAssetDistance = 1.5f;
                if (Vector3.Distance(new Vector3(pos.x, 0, pos.z), new Vector3(child.position.x, 0, child.position.z)) < minAssetDistance)
                {
                    return false;
                }
            }

            return true;
        };

        SpawnEnemies(info.enemyCount, room, planeWidth, planeHeight, wallStyle.ToStringValue(), enemyPositionFilter);

        // Room finalization of info object, if present
        if (info.finalizeRoom != null)
        {
            info.finalizeRoom(room);
        }

        return room;
    }

    private static void SpawnEnemies(int enemyCount, GameObject room, float planeWidth, float planeHeight, string theme, System.Func<Vector3, bool> positionFilter = null)
    {
        // Load enemy prefabs from Resources folder
        List<string> enemyNames = new List<string>();
        switch (theme)
        {
            case "Forest":
                enemyNames.Add("GreenBlobPrefab");
                enemyNames.Add("BlueBlobPrefab");
                enemyNames.Add("SnakePrefab");
                break;
            case "Dungeon":
                enemyNames.Add("RedBlobPrefab");
                enemyNames.Add("VioletBlobPrefab");
                enemyNames.Add("BatPrefab");
                break;
            case "Cave":
                enemyNames.Add("RedBlobPrefab");
                enemyNames.Add("VioletBlobPrefab");
                enemyNames.Add("BatPrefab");
                break;
            default:
                Debug.LogError($"Unknown theme: {theme}");
                return;
        }
        Debug.Log($"Spawning {enemyCount} enemies of theme {theme} in room {room.name}");
        Debug.Log($"Enemy names: {string.Join(", ", enemyNames)}");

        // Load all prefabs
        List<GameObject> enemyPrefabs = new List<GameObject>();
        foreach (string enemyName in enemyNames)
        {
            GameObject prefab = Resources.Load<GameObject>($"Characters/{enemyName}");
            if (prefab != null)
            {
                enemyPrefabs.Add(prefab);
            }
            else
            {
                Debug.LogError($"Enemy prefab not found: {enemyName}");
            }
        }

        // Create enemies
        for (int i = 0; i < enemyCount; i++)
        {
            // Select a random prefab from the list
            GameObject selectedPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Count)];

            // Try up to 50 positions until we find a valid one
            Vector3 position = Vector3.zero;
            bool foundValidPosition = false;
            int attempts = 0;
            int maxAttempts = 50;

            if (positionFilter != null)
            {
                // Keep trying positions until we find one that passes the filter
                while (!foundValidPosition && attempts < maxAttempts)
                {
                    position = GetRandomPosition(1.5f, planeWidth, planeHeight);
                    foundValidPosition = positionFilter(position);
                    attempts++;
                }

                if (!foundValidPosition)
                {
                    Debug.LogWarning($"Failed to find valid position for enemy {i + 1} after {maxAttempts} attempts. Using last attempted position.");
                }
            }
            else
            {
                // No filter function provided, use default position generation
                position = GetRandomPosition(1.5f, planeWidth, planeHeight);
            }

            // Create an instance of the prefab
            GameObject enemyInstance = Object.Instantiate(selectedPrefab);
            Debug.Log($"Spawned Enemy of prefab: {enemyInstance.name}");

            // Set position and rotation
            enemyInstance.transform.position = position;
            enemyInstance.transform.rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);

            // Parent the enemy to the room
            enemyInstance.transform.parent = room.transform;

            // Name the enemy
            enemyInstance.name = $"Enemy_{selectedPrefab.name}_{i + 1}";
        }
    }

    private static GameObject CreateDoor(float wallPos, float planeWidth, float planeHeight, RoomStyle wallStyle, string theme = "All")
    {
        // wallId is floor of wallPos
        int wallId = Mathf.FloorToInt(wallPos);
        // Load door prefab from Resources folder
        string doorPrefabPath = $"Rooms/{theme}/Doors";
        GameObject[] doorPrefabs = Resources.LoadAll<GameObject>(doorPrefabPath);
        GameObject doorPrefab = doorPrefabs[Random.Range(0, doorPrefabs.Length)];
        GameObject doorInstance = Object.Instantiate(doorPrefab);
        doorInstance.name = $"Door_{wallId + 1}";

        doorInstance.transform.rotation = Quaternion.Euler(0f, 90f * wallId, 0f); // Reset rotation

        // If only wall side is specified and not exact position, use random factor between -0.25 and +0.25
        float doorPosOnWall = wallPos - wallId;
        if (doorPosOnWall == 0)
        {
            doorPosOnWall = Random.Range(-0.25f, 0.25f);
        }
        else
        {
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

    private static Material GetWallMaterialForStyle(Material baseMaterial, RoomStyle style)
    {
        return AddRandomTextureToBaseRoomMaterial(baseMaterial, style, "wall");
    }

    private static Material GetFloorMaterialForStyle(Material baseMaterial, RoomStyle style)
    {
        return AddRandomTextureToBaseRoomMaterial(baseMaterial, style, "floor");
    }

    private static Material AddRandomTextureToBaseRoomMaterial(Material baseMaterial, RoomStyle style, string spriteFilter)
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
        // if (isTransparent)
        // {
        //     // enable alpha clipping
        //     material.SetFloat("_AlphaClip", 1);
        //     material.SetFloat("_Cutoff", 0.5f);
        //     material.EnableKeyword("_ALPHATEST_ON");
        // }
        // Repeat the texture if floor splite contains 'Tile' in name
        if (spriteFilter == "floor" && randomSprite.name.ToLower().Contains("tile"))
        {
            // This is awkward but, uh, we just "mark" this as "should be repeated" with negative numbers, because we don't know the total width/height here...
            // We provide the texture size in pixels here for later access
            Debug.Log($"Floor texture size: {randomSprite.texture.width}x{randomSprite.texture.height}");
            material.mainTextureScale = new Vector2(-1, -1);
        }
        return material;
    }

    private static void CreateThemedDecoElements(RoomStyle[] RoomStyles, int decoCount, GameObject room, float planeWidth, float planeHeight)
    {
        // List to store all eligible decoration prefabs
        List<GameObject> eligiblePrefabs = new List<GameObject>();

        // Load prefabs for each style in RoomStyles
        foreach (RoomStyle style in RoomStyles)
        {
            string resourcePath = $"Rooms/{style.ToStringValue()}/Deco";
            GameObject[] prefabs = Resources.LoadAll<GameObject>(resourcePath);

            Debug.Log($"Loaded {prefabs.Length} decoration prefabs from {resourcePath}");

            // Add all prefabs to the eligible list
            foreach (GameObject prefab in prefabs)
            {
                eligiblePrefabs.Add(prefab);
            }
        }

        // If no prefabs found, log error and return
        if (eligiblePrefabs.Count == 0)
        {
            Debug.LogError("No decoration prefabs found for the specified styles.");
            return;
        }

        // Create the specified number of decoration elements
        for (int i = 0; i < decoCount; i++)
        {
            // Select a random prefab from the eligible list
            GameObject selectedPrefab = eligiblePrefabs[Random.Range(0, eligiblePrefabs.Count)];
            // Create an instance of the prefab
            GameObject decoInstance = Object.Instantiate(selectedPrefab);
            // Set a random position within the room
            decoInstance.transform.position = GetRandomPosition(1.5f, planeWidth, planeHeight);
            // Randomly rotate the decoration around the Y axis
            decoInstance.transform.rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
            // Parent the decoration to the room
            decoInstance.transform.parent = room.transform;
            // Name the decoration
            decoInstance.name = $"Deco_{selectedPrefab.name}_{i + 1}";
            // Add slight random scale variation for visual interest (optional)
            // float scaleVariation = Random.Range(0.8f, 1.2f);
            // decoInstance.transform.localScale *= scaleVariation;
        }
    }
}
