using UnityEngine;

public class GenericEnemyController : MonoBehaviour
{
    public float moveSpeed = 2f; // Speed at which the enemy moves
    public float rotationSpeed = 5f; // Speed at which the enemy rotates to face the player
    public float wobbleAmplitude = 0.5f; // Amplitude of the wobble (height of the up-and-down motion)
    public float wobbleFrequency = 2f; // Frequency of the wobble (speed of the up-and-down motion)

    private Transform playerTransform; // Reference to the player's transform
    private float initialY; // The initial Y position of the enemy
    private Rigidbody rb; // Reference to the Rigidbody component

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

        // Get the Rigidbody component
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("GenericEnemyController: No Rigidbody component found on the enemy!");
        }
    }

    void FixedUpdate()
    {
        if (playerTransform == null || rb == null) return;

        // Calculate the direction to the player
        Vector3 directionToPlayer = (playerTransform.position - transform.position).normalized;

        // Add wobble effect to the Y position
        float wobbleOffset = Mathf.Sin(Time.time * wobbleFrequency) * wobbleAmplitude;
        Vector3 velocity = directionToPlayer * moveSpeed;
        velocity.y = wobbleOffset;

        // Apply velocity to the Rigidbody
        rb.velocity = velocity;

        // Smoothly rotate to face the same direction as the player
        Quaternion targetRotation = playerTransform.rotation;
        rb.MoveRotation(Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime));
    }
}