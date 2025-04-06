using System.Collections;
using UnityEngine;

public class GenericEnemyController : MonoBehaviour
{
    public float moveSpeed = 2f; // Speed at which the enemy moves
    public float rotationSpeed = 5f; // Speed at which the enemy rotates to face the player
    public float wobbleAmplitude = 0.5f; // Amplitude of the wobble (height of the up-and-down motion)
    public float wobbleFrequency = 2f; // Frequency of the wobble (speed of the up-and-down motion)
    public float fireInterval = 2f; // Time between shots
    private Transform playerTransform; // Reference to the player's transform
    private float initialY; // The initial Y position of the enemy
    private Rigidbody rb; // Reference to the Rigidbody component
    private SpriteRenderer spriteRenderer; // Reference to the enemy's SpriteRenderer

    public bool spriteIsActivating = false;

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

        // Get the SpriteRenderer component
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            // Make the enemy invisible initially
            spriteRenderer.enabled = false;
        }

        StartCoroutine(FireBullets());

    }

    void FixedUpdate()
    {
        if (GameManager.Instance.CurrentState != GameManager.GameState.LevelStart)
        {
            return;
        }

        // Make the enemy visible when the level starts
        if (spriteRenderer != null && !spriteRenderer.enabled && !spriteIsActivating)
        {
            spriteIsActivating = true; // Prevent multiple calls to the coroutine
            StartCoroutine(EnableSpriteRendererWithDelay());
        }
        if (spriteRenderer.enabled)
        {
            MoveTowardsPlayer();
        }
    }

    private IEnumerator EnableSpriteRendererWithDelay()
    {
        // Wait for a random delay between 0 and 1 second
        float randomDelay = Random.Range(0f, 1f);
        yield return new WaitForSeconds(randomDelay);

        // Enable the SpriteRenderer
        spriteRenderer.enabled = true;
    }

    private void MoveTowardsPlayer()
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
    
    private IEnumerator FireBullets()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(fireInterval, fireInterval + 1f));

            if (GameManager.Instance.CurrentState == GameManager.GameState.LevelStart)
            {
                FireBullet();
            }
        }
    }

  private void FireBullet()
    {
        GameObject bulletPrefab = Resources.Load<GameObject>("GenericBulletPrefab"); // Load the bullet prefab from Resources
        if (bulletPrefab != null && playerTransform != null)
        {
            // Calculate the direction to the player
            Vector3 directionToPlayer = (playerTransform.position - transform.position).normalized;

            // Instantiate the bullet at the enemy's center
            GameObject bullet = Instantiate(bulletPrefab, transform.position, Quaternion.LookRotation(directionToPlayer));
        }
    }
}