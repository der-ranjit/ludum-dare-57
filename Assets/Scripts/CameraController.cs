using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target; // The player or object the camera will follow
    public Vector3 offset = new Vector3(0f, 1.5f, -3.5f); // Fixed offset relative to the player's forward direction

    private Vector3 overridePos = Vector3.zero; // Override position for the camera
    private bool overrideActive = false; // Flag to indicate if the override is active
    // public Quaternion overrideAngle = Quaternion.identity; // Override angle for the camera
    // public Quaternion overrideTargetAngle = Quaternion.identity; // Override target angle for the camera
    private float overrideAlpha = 0f;

    private Vector3 overrideTarget = Vector3.zero; // Override target position for the camera
    private Vector3 overrideCurrentTarget = Vector3.zero; // Current target position for the camera, this always moves smoothly towards overrideTarget

    void LateUpdate()
    {
        if (GameManager.Instance?.CurrentState != GameManager.GameState.Playing)
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
        // Vector3 targetPosition = target.position + offset;
        targetPosition.y = Mathf.Max(targetPosition.y, 0.5f); // Keep the camera at the same height as the player
        transform.position = targetPosition;

        // If override is active, set the camera position to the override position
        if (overrideActive) {
            overrideAlpha = Mathf.Clamp01(overrideAlpha + Time.deltaTime * 2f);
        }
        else
        {
            overrideAlpha = Mathf.Clamp01(overrideAlpha - Time.deltaTime * 2f);
        }
        if (overrideAlpha > 0f)
        {
            float smoothAlpha = 0.5f - 0.5f * Mathf.Cos(overrideAlpha * Mathf.PI);
            transform.position = Vector3.Lerp(transform.position, overridePos, smoothAlpha);
            // transform.rotation = Quaternion.Slerp(transform.rotation, overrideAngle, overrideAlpha);
            // transform.LookAt(target.position + target.forward * offset.z + target.up * offset.y + target.right * offset.x);
        }
        else
        {
            // Otherwise, set the camera position to the calculated position
            transform.position = targetPosition;
            overrideCurrentTarget = target.position + offset;
        }

        // Always smoothly move overrideCurrentTarget towards overrideTarget
        overrideCurrentTarget = Vector3.Lerp(overrideCurrentTarget, overrideTarget, Time.deltaTime * 5f);

        // Always look at the target
        Vector3 targetToLookAt = target.position;
        if (overrideAlpha > 0) {
            targetToLookAt = (1 - overrideAlpha) * targetToLookAt + overrideAlpha * overrideCurrentTarget;
        } 
        transform.LookAt(targetToLookAt);
    }

    public void SetOverride(Vector3? newPos = null)
    {
        if (newPos != null)
        {
            overridePos = (Vector3)newPos;
            overrideActive = true;
        }
        else
        {
            overrideActive = false;
        }
    }

    public void SetOverrideTarget(Vector3? newTarget = null)
    {
        if (newTarget != null)
        {
            overrideTarget = (Vector3)newTarget;
        }
        else
        {
            overrideTarget = target.position;
        }
    }


}