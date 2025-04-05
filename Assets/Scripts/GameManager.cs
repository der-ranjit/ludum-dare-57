using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public enum GameState
    {
        PreStart, // Enemies are turned and cannot move
        LevelStart // Normal gameplay begins
    }

    public GameState CurrentState { get; private set; } = GameState.PreStart;

    public float preStartDuration = 5f; // Duration of the PreStart state
    public int enemyCount = 5; // Number of enemies to spawn
    public float noSpawnRadius = 10f; // Radius around the center of the room to spawn enemies

    public float cameraHeightOffset = 10f; // Adjustable height offset for the top-down view
    public float cameraTransitionDuration = 3f; // Duration of the camera transition

    private Camera mainCamera;

    public void SetState(GameState newState)
    {
        CurrentState = newState;
        Debug.Log($"Game state changed to: {newState}");
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

        // Generate the room
        GameObject room = RoomCreator.GenerateRoom(null, 20f, 20f, null);
        SpawnEnemies(room, enemyCount, noSpawnRadius);

        // Set the camera to a top-down view
        SetCameraToTopDownView(room);

        // Start the transition to LevelStart
        StartCoroutine(TransitionToLevelStart());
    }

    private static void SpawnEnemies(GameObject room, int spawnCount, float noSpawnRadius)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player"); // Find the player object
        GameObject genericEnemyPrefab = Resources.Load<GameObject>("GenericEnemyPrefab"); // Load the enemy prefab from Resources
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
    SetState(GameState.LevelStart);

    // Enable enemy movement
    GenericEnemyController[] enemies = FindObjectsOfType<GenericEnemyController>();
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
}