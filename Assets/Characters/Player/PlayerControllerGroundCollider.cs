using UnityEngine;

public class GroundCollider : MonoBehaviour
{
    private PlayerController playerController;

    void Start()
    {
        playerController = GetComponentInParent<PlayerController>();
    }

    void OnTriggerStay(Collider other)
    {
        if (other.gameObject != transform.parent.gameObject) // Ignore collisions with the player itself
        {
            playerController.SetGroundedState(true);
            // Check if the other object has GenericEnemyController script, if so, auto jump
            EnemyController enemyController = other.GetComponent<EnemyController>();
            if (enemyController != null)
            {
                playerController.JumpFromEnemy();
                // TODO: damage enemy
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject != transform.parent.gameObject) // Ignore collisions with the player itself
        {
            playerController.SetGroundedState(false);
        }
    }
}