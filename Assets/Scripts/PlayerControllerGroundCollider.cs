using UnityEngine;

public class GroundCollider : MonoBehaviour
{
    private PlayerController playerController;

    void Start()
    {
        playerController = GetComponentInParent<PlayerController>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject != transform.parent.gameObject) // Ignore collisions with the player itself
        {
            playerController.SetGroundedState(true);
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