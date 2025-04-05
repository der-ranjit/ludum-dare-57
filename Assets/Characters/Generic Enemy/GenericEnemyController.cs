using UnityEngine;

public class GenericEnemyController : MonoBehaviour
{
    public float moveSpeed = 2f; // Speed at which the enemy moves
    public float rotationSpeed = 5f; // Speed at which the enemy rotates to face the player
    public float wobbleAmplitude = 0.5f; // Amplitude of the wobble (height of the up-and-down motion)
    public float wobbleFrequency = 2f; // Frequency of the wobble (speed of the up-and-down motion)

    private Transform playerTransform; // Reference to the player's transform
    private float initialY; // The initial Y position of the enemy

    void Start()
    {
        // Find the player by tag
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }
        else
        {
            Debug.LogWarning("GenericEnemyController: No player found with tag 'Player'!");
        }

        // Store the initial Y position
        initialY = transform.position.y;
    }

    void Update()
    {
        if (playerTransform == null) return;

        // Move toward the player
        Vector3 directionToPlayer = (playerTransform.position - transform.position).normalized;
        Vector3 targetPosition = transform.position + directionToPlayer * moveSpeed * Time.deltaTime;

        // Add wobble effect to the Y position
        targetPosition.y = initialY + Mathf.Sin(Time.time * wobbleFrequency) * wobbleAmplitude;

        // Update the enemy's position
        transform.position = targetPosition;

        // Smoothly rotate to face the same direction as the player
        Quaternion targetRotation = playerTransform.rotation;
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }
}