using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target; // The player or object the camera will follow
    public Vector3 offset = new Vector3(0f, 1.5f, -3.5f); // Fixed offset from the target

    void LateUpdate()
    {
        target = GameObject.FindGameObjectWithTag("Player")?.transform; // Find the player object by tag
        if (target == null)
        {
            Debug.LogWarning("CameraController: No target assigned!");
            return;
        }
        transform.position = target.position + offset;

        // Always look at the target
        transform.LookAt(target);
    }
}