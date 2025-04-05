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
        // Generate the room
        GameObject room = RoomCreator.GenerateRoom(null, 20f, 20f, null);
        SpawnEnemies(room, enemyCount, noSpawnRadius);
        StartCoroutine(TransitionToLevelStart());
    }

    private static void SpawnEnemies(GameObject room, int enemyCount, float noSpawnRadius)
    {
        GameObject genericEnemyPrefab = Resources.Load<GameObject>("GenericEnemyPrefab"); // Load the enemy prefab from Resources
        for (int i = 0; i < enemyCount; i++)
        {
            Vector3 spawnPosition = Random.insideUnitSphere * noSpawnRadius;
            // TODO change based on floating or grounded enemy 
            spawnPosition.y = 0.3f;

            GameObject enemy = Object.Instantiate(genericEnemyPrefab, spawnPosition, Quaternion.identity);
            enemy.name = $"Enemy_{i + 1}";
            enemy.transform.parent = room.transform;

            // Set initial rotation (90 degrees left or right)
            float randomAngle = Random.value > 0.5f ? 90f : -90f;
            enemy.transform.Rotate(Vector3.up, randomAngle);
        }
    }

    private IEnumerator TransitionToLevelStart()
    {
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
}