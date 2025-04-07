using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public enum GameState
    {
        PreStart, // Enemies are turned and cannot move
        Playing // Normal gameplay begins
    }
    public GameObject playerPrefab;

    [SerializeField]
    public GameState CurrentState = GameState.PreStart;
    [Header("Debug")]
    public bool preventRoomCreation = false; // Prevent room creation for debugging purposes

    [Header("Room Start Settings")]
    public float preStartDuration = 5f; // Duration of the PreStart state
    public int enemyCount = 5; // Number of enemies to spawn
    public int killedEnemies = 0;

    public float noSpawnRadius = 10f; // Radius around the center of the room to spawn enemies

    public float cameraHeightOffset = 10f; // Adjustable height offset for the top-down view
    public float cameraTransitionDuration = 3f; // Duration of the camera transition

    private float proceedToNextRoomTimer = 0f; // Timer for proceeding to the next room
    private float proceedToNextRoomDuration = 1f;
    private float timeSpentInCurrentRoom = 0f; // Time spent in the current room
    private float fadeInDuration = 0.5f; // Duration for fading in

    private Camera mainCamera;

    public void SetState(GameState newState)
    {
        CurrentState = newState;
        Debug.Log($"Game state changed to: {newState}");
        Debug.Log($"Game state changed to: {newState}");
    }

    public void Update()
    {
        timeSpentInCurrentRoom += Time.deltaTime;
        if (proceedToNextRoomTimer > 0f)
        {
            proceedToNextRoomTimer -= Time.deltaTime;
            if (proceedToNextRoomTimer <= 0f)
            {
                ProceedToNextRoom();
            }
        }
        // Fading
        float fadeOutAlpha = proceedToNextRoomTimer > 0 ? 1 - (proceedToNextRoomTimer / proceedToNextRoomDuration) : 0;
        float fadeInAlpha = 1 - (timeSpentInCurrentRoom / fadeInDuration);
        float alpha = Mathf.Clamp01(Mathf.Max(fadeOutAlpha, fadeInAlpha));
        ScreenFader.Instance?.setCurrentFadeAlpha(alpha);
    }

    public void EnemyKilled()
    {
        killedEnemies++;
        UIManager.Instance.UpdateEnemyCount(killedEnemies, enemyCount);

        if (killedEnemies >= enemyCount)
        {
            Debug.Log("All enemies killed!");
            killedEnemies = 0; // Reset killed enemies for the next level
            // Handle level completion
        }
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // Persist across scenes
    }

    private void Start()
    {
        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("GameManager: No main camera found!");
            return;
        }
        if (preventRoomCreation)
        {
            Debug.Log("Room creation is prevented for debugging purposes.");
            return;
        }
        StartCoroutine(CreateNextRoom());
    }

    private IEnumerator CreateNextRoom()
    {
        SetState(GameState.PreStart);

        float width = Random.Range(10f, 25f);
        float height = Random.Range(10f, 25f);
        Debug.Log($"Creating room with dimensions: {width} x {height}");
        int enemies = 1; // Random.Range(1, 10);
        GameObject room = RoomCreator.DeleteAndGenerateRoom(null, width, height, null, playerPrefab);
        SpawnEnemies(room, enemies, noSpawnRadius);

        // Set the camera to a top-down view
        SetCameraToTopDownView(room);

        // Start the transition to LevelStart
        yield return StartCoroutine(TransitionToLevelStart());
    }

    public void ReplayRoomIn(float delay) {
        RoomCreator.StayInRoom();
        ProceedToNextRoomIn(delay);
    }

    public void ProceedToNextRoomIn(float delay)
    {
        if (proceedToNextRoomTimer > 0f)
        {
            return; // Already in the process of proceeding to the next room
        }
        proceedToNextRoomTimer = delay;
        proceedToNextRoomDuration = delay;
    }

    private static void SpawnEnemies(GameObject room, int spawnCount, float noSpawnRadius)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player"); // Find the player object
        GameObject genericEnemyPrefab = Resources.Load<GameObject>("Characters/GenericEnemyPrefab"); // Load the enemy prefab from Resources
        GameObject plane = room.transform.Find("Floor").gameObject; // Find the plane in the room
        Vector3 planeCenter = plane.transform.position;
        Vector3 planeSize = plane.transform.localScale * 10f; // Unity's default plane is 10x10 units
        int spawned = 0;
        int attempts = 0;
        int maxAttempts = spawnCount * 10;
        while (spawned < spawnCount && attempts < maxAttempts)
        {
            attempts++;

            // Generate random position within plane bounds
            float randX = Random.Range(-planeSize.x / 2f, planeSize.x / 2f);
            float randZ = Random.Range(-planeSize.z / 2f, planeSize.z / 2f);
            // TODO change based on floating or grounded enemy 
            float spawnOffsetY = 0.25f;
            Vector3 spawnPosition = new Vector3(planeCenter.x + randX, planeCenter.y + spawnOffsetY, planeCenter.z + randZ);

            // Avoid player radius
            if (Vector3.Distance(spawnPosition, player.transform.position) >= noSpawnRadius)
            {
                GameObject enemy = Object.Instantiate(genericEnemyPrefab, spawnPosition, Quaternion.identity);
                enemy.name = $"Enemy_{spawned + 1}";
                enemy.transform.parent = room.transform;

                // Set the enemy's rotation to face away from the camera, essentially making it invisible
                // Calculate the direction from the enemy to the camera
                Vector3 directionToCamera = (Camera.main.transform.position - enemy.transform.position).normalized;
                // Randomly decide whether to invert the rotation
                bool invertRotation = Random.value > 0.5f;

                // Align the enemy's right vector to point at the camera (or its inverse)
                Vector3 rightVector = Vector3.Cross(Vector3.up, directionToCamera);
                if (invertRotation)
                {
                    rightVector = -rightVector; // Invert the right vector
                }

                Quaternion invisibleRotation = Quaternion.LookRotation(rightVector, Vector3.up);
                enemy.transform.rotation = invisibleRotation;

                spawned++;
            }
        }

    }

    public bool IsRoomComplete()
    {
        if (CurrentState != GameState.Playing)
        {
            return false;
        }
        // Count the number of enemies in the scene
        Enemy[] enemies = FindObjectsOfType<Enemy>();
        int enemyCount = enemies.Length;
        // Check if all enemies are defeated
        return enemyCount < 2;
    }

    // Regular method that can be called directly
    private void ProceedToNextRoom()
    {
        proceedToNextRoomTimer = 0f; // Reset the timer
        timeSpentInCurrentRoom = 0f; // Reset the timer
        StartCoroutine(ProceedToNextRoomCoroutine());
    }

    // Internal coroutine that handles the actual work
    private IEnumerator ProceedToNextRoomCoroutine()
    {
        // Delay to next frame
        yield return null;

        // Then create the next room
        yield return StartCoroutine(CreateNextRoom());
    }

    private IEnumerator TransitionToLevelStart()
    {
        Transform player = GameObject.FindGameObjectWithTag("Player")?.transform; // Find the player object by tag
        Vector3 offset = mainCamera.GetComponent<CameraController>().offset;

        Vector3 playerPositionAfterSpwan = player.position - (player.up * 0.5f); // Adjust the player's position to account for the spawn offset 
                                                                                 // Calculate the target camera position and rotation
        Vector3 targetPlayerCameraPosition = playerPositionAfterSpwan
            + (player.right * offset.x) // Horizontal offset
            + (player.up * offset.y) // Vertical offset + initial player spawn offset
            + (player.forward * offset.z); // Depth offset
                                           // log
        Debug.Log($"Target Camera Position: {targetPlayerCameraPosition}");

        Quaternion targetPlayerCameraRotation = Quaternion.LookRotation(playerPositionAfterSpwan - targetPlayerCameraPosition);

        // Transition the camera to the player's position
        yield return StartCoroutine(TransitionCameraToPlayer(targetPlayerCameraPosition, targetPlayerCameraRotation));
        yield return new WaitForSeconds(preStartDuration);

        // Change the game state to LevelStart
        SetState(GameState.Playing);

        // Enable enemy movement
        Enemy[] enemies = FindObjectsOfType<Enemy>();
        foreach (var enemy in enemies)
        {
            enemy.enabled = true;
        }
    }

    private void SetCameraToTopDownView(GameObject room)
    {
        // Get the room's floor plane
        GameObject plane = room.transform.Find("Floor").gameObject;
        if (plane == null)
        {
            Debug.LogError("GameManager: No floor plane found in the room!");
            return;
        }

        // Calculate the center and size of the plane
        Vector3 planeCenter = plane.transform.position;
        Vector3 planeSize = plane.transform.localScale * 10f; // Unity's default plane is 10x10 units

        // Position the camera above the center of the plane
        float maxDimension = Mathf.Max(planeSize.x, planeSize.z);
        float cameraHeight = maxDimension + cameraHeightOffset; // Adjust height based on the largest dimension
        mainCamera.transform.position = new Vector3(planeCenter.x, cameraHeight, planeCenter.z);

        // Rotate the camera to look straight down
        mainCamera.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
    }

    private IEnumerator TransitionCameraToPlayer(Vector3 targetPosition, Quaternion targetRotation)
    {
        float elapsedTime = 0f;

        Vector3 startPosition = mainCamera.transform.position;
        Quaternion startRotation = mainCamera.transform.rotation;

        while (elapsedTime < cameraTransitionDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / cameraTransitionDuration;

            // Interpolate position and rotation
            mainCamera.transform.position = Vector3.Lerp(startPosition, targetPosition, t);
            mainCamera.transform.rotation = Quaternion.Slerp(startRotation, targetRotation, t);

            yield return null;
        }

        // Ensure the camera ends exactly at the original position and rotation
        mainCamera.transform.position = targetPosition;
        mainCamera.transform.rotation = targetRotation;
    }

    public void SpawnPowerUpsInRoom(GameObject room, int count)
    {
        GameObject floor = room.transform.Find("Floor").gameObject;
        Vector3 roomCenter = floor.transform.position;
        Vector3 roomSize = floor.transform.localScale * 10f;

        for (int i = 0; i < count; i++)
        {
            float randX = Random.Range(-roomSize.x / 2f, roomSize.x / 2f);
            float randZ = Random.Range(-roomSize.z / 2f, roomSize.z / 2f);
            Vector3 spawnPosition = new Vector3(roomCenter.x + randX, roomCenter.y + 0.5f, roomCenter.z + randZ);

            SpawnRandomPowerUp(spawnPosition);
        }
    }

    private void SpawnRandomPowerUp(Vector3 position)
    {
        GameObject powerUpPrefab = Resources.Load<GameObject>("PowerUps/PowerUpPrefab");
        if (powerUpPrefab != null)
        {
            Instantiate(powerUpPrefab, position, Quaternion.identity);
        }
    }
}