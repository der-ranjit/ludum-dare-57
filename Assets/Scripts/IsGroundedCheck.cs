using UnityEngine;

public class IsGroundedCheck : MonoBehaviour
{
    private bool isGrounded = false;

    public bool IsGrounded()
    {
        return isGrounded;
    }

    void Start()
    {
        // ground checks should be attached to a character's GroundCollider child
        bool isCharacter = transform.parent != null && (transform.parent.gameObject.CompareTag("Player") || transform.parent.gameObject.CompareTag("Enemy"));
        if (!isCharacter)
        {
            Debug.LogError("GroundCollider must be a attached to a child of a character object.");
        }
    }

    void OnTriggerStay(Collider other)
    {

        if (other.gameObject != transform.parent.gameObject) // Ignore collisions with the owner
        {
            isGrounded = true;
        }
        Enemy enemyController = other.GetComponent<Enemy>();
        PlayerController playerController = transform.parent.GetComponent<PlayerController>();
        if (enemyController != null && playerController != null)
        {
            playerController.JumpFromEnemy();
            enemyController.TakeDamage(playerController.powerUpStats.damage);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject != transform.parent.gameObject) // Ignore collisions with the owner
        {
            isGrounded = false;
        }
    }
}