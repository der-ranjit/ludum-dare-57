using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SlitVictim))]
[RequireComponent(typeof(Rigidbody))]
public class Enemy : MonoBehaviour, IDamageable
{

    public float maxHealth = 2f;
    private float currentHealth;
    public float moveSpeed = 1f; // Speed at which the enemy moves
    public float rotationSpeed = 1.5f; // Speed at which the enemy rotates to face the player
    protected Transform playerTransform; // Reference to the player's transform
    protected Transform playerHeadAimPoint;
    protected Rigidbody rb; // Reference to the Rigidbody component
    protected SpriteRenderer spriteRenderer; // Reference to the enemy's SpriteRenderer
    protected Weapon weapon;
    private bool spriteIsActivating = false;

    protected virtual void Start()
    {
        currentHealth = maxHealth;

        // Find the player by tag
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
            playerHeadAimPoint = player.transform.Find("PlayerHeadAimPoint");
            // disable collision between capsule collider and player collider
            CapsuleCollider enemyCollider = GetComponent<CapsuleCollider>();
            BoxCollider playerCollider = player.GetComponent<BoxCollider>();
            if (enemyCollider != null && playerCollider != null)
            {
                Physics.IgnoreCollision(enemyCollider, playerCollider);
            }
        }
        else
        {
            Debug.LogWarning("GenericEnemyController: No player found with tag 'Player'!");
        }

        // Get the Rigidbody component
        rb = GetComponent<Rigidbody>();

        // Get the SpriteRenderer component
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            // Make the enemy invisible initially
            spriteRenderer.enabled = false;
        }

        weapon = GetComponentInChildren<Weapon>();
    }

    protected virtual void Update()
    {
        if (GameManager.Instance?.CurrentState != GameManager.GameState.Playing) return;

        // Make the enemy visible when the level starts
        if (spriteRenderer != null && !spriteRenderer.enabled && !spriteIsActivating)
        {
            spriteIsActivating = true; // Prevent multiple calls to the coroutine
            StartCoroutine(EnableSpriteRendererWithDelay());
        }
        if (spriteRenderer.enabled)
        {
            MoveTowardsPlayer();
            RotateTowardsPlayer();
        }
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        StartCoroutine(FlashRed());
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        GameManager.Instance.EnemyKilled();
        Destroy(gameObject);
    }

    private IEnumerator EnableSpriteRendererWithDelay()
    {
        // Wait for a random delay between 0 and 1 second
        float randomDelay = Random.Range(0f, 1f);
        yield return new WaitForSeconds(randomDelay);

        // Enable the SpriteRenderer
        spriteRenderer.enabled = true;
    }

    protected void MoveTowardsPlayer()
    {
        if (playerTransform == null || rb == null) return;

        Vector3 directionToPlayer = (playerTransform.position - transform.position).normalized;
        rb.velocity = directionToPlayer * moveSpeed;

    }

    protected void RotateTowardsPlayer()
    {
        if (playerTransform == null || rb == null) return;

        // Smoothly rotate to face the same direction as the player
        Quaternion targetRotation = playerTransform.rotation;
        rb.MoveRotation(Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime));
    }

    protected void Attack()
    {
        Transform aimPoint = playerHeadAimPoint != null ? playerHeadAimPoint : playerTransform;
        Vector3 direction = (aimPoint.position - transform.position).normalized;
        weapon?.Attack(direction);
    }

    private IEnumerator FlashRed()
    {
        if (spriteRenderer == null) yield break;
        // Get the material and save the original color
        Material material = spriteRenderer.material;

        // Set the color to yellow
        material.SetColor("_TintColor", Color.yellow);
        // Wait for a short duration
        yield return new WaitForSeconds(0.3f);
        // Revert to the original color
        material.SetColor("_TintColor", Color.white);
    }
}