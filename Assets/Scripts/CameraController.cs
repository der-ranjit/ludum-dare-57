using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target; // The player or object the camera will follow
    public Vector3 offset = new Vector3(0f, 1.5f, -3.5f); // Fixed offset relative to the player's forward direction

    void LateUpdate()
    {
        if (GameManager.Instance.CurrentState != GameManager.GameState.LevelStart)
        {
            // let level camera transition control the camera
            return;
        }

        target = GameObject.FindGameObjectWithTag("Player")?.transform; // Find the player object by tag
        if (target == null)
        {
            Debug.LogWarning("CameraController: No target assigned!");
            return;
        }

        // Calculate the camera's position relative to the player's forward direction
        Vector3 targetPosition = target.position + target.forward * offset.z + target.up * offset.y + target.right * offset.x;
        transform.position = targetPosition;

        // Always look at the target
        transform.LookAt(target);
    }
}