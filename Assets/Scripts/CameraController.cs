using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target; // The player or object the camera will follow
    public Vector3 offset = new Vector3(0f, 3f, -5f); // Offset from the target
    public float followSpeed = 5f; // Speed at which the camera follows the target

    public float rotationSpeed = 100f; // Speed of camera rotation
    public float minVerticalAngle = -30f; // Minimum vertical angle
    public float maxVerticalAngle = 60f; // Maximum vertical angle
    public float minHorizontalAngle = -45f; // Minimum horizontal angle (relative to forced perspective)
    public float maxHorizontalAngle = 45f; // Maximum horizontal angle (relative to forced perspective)

    private float currentYaw = 0f; // Current horizontal rotation
    private float currentPitch = 0f; // Current vertical rotation

    public bool isForcedPerspective = true; // Toggle between forced perspective and free camera
    public Vector3 forcedPerspectiveRotation = new Vector3(30f, 0f, 0f); // Fixed rotation for forced perspective

    void LateUpdate()
    {
        target = GameObject.FindGameObjectWithTag("Player")?.transform; // Find the player object by tag
        if (target == null)
        {
            Debug.LogWarning("CameraController: No target assigned!");
            return;
        }

        if (isForcedPerspective)
        {
            HandleForcedPerspective();
        }
        else
        {
            HandleFreeCamera();
        }
    }

    void Update()
    {
        // Toggle between forced perspective and free camera with a key press (e.g., "C")
        if (Input.GetKeyDown(KeyCode.C))
        {
            isForcedPerspective = !isForcedPerspective;

            if (isForcedPerspective)
            {
                // Reset camera to focus on the player when switching back to forced perspective
                currentYaw = forcedPerspectiveRotation.y;
                currentPitch = forcedPerspectiveRotation.x;
            }
        }
    }

    private void HandleForcedPerspective()
    {
        // Allow tilting up and down, and looking to the sides
        float horizontalInput = Input.GetAxis("Mouse X");
        float verticalInput = Input.GetAxis("Mouse Y");

        currentYaw += horizontalInput * rotationSpeed * Time.deltaTime;
        currentPitch -= verticalInput * rotationSpeed * Time.deltaTime;

        // Clamp vertical and horizontal rotation
        currentPitch = Mathf.Clamp(currentPitch, minVerticalAngle, maxVerticalAngle);
        currentYaw = Mathf.Clamp(currentYaw, forcedPerspectiveRotation.y + minHorizontalAngle, forcedPerspectiveRotation.y + maxHorizontalAngle);

        // Smoothly follow the target
        Vector3 targetPosition = target.position + offset;
        transform.position = Vector3.Lerp(transform.position, targetPosition, followSpeed * Time.deltaTime);

        // Apply rotation with adjustable pitch and yaw
        transform.rotation = Quaternion.Euler(currentPitch, currentYaw, 0f);
    }

    private void HandleFreeCamera()
    {
        // Orbit around the player based on mouse input
        float horizontalInput = Input.GetAxis("Mouse X");
        float verticalInput = Input.GetAxis("Mouse Y");

        currentYaw += horizontalInput * rotationSpeed * Time.deltaTime;
        currentPitch -= verticalInput * rotationSpeed * Time.deltaTime;
        currentPitch = Mathf.Clamp(currentPitch, minVerticalAngle, maxVerticalAngle);

        // Calculate new camera position
        Quaternion rotation = Quaternion.Euler(currentPitch, currentYaw, 0f);
        Vector3 newPosition = target.position + rotation * offset;

        // Smoothly move the camera to the new position
        transform.position = Vector3.Lerp(transform.position, newPosition, followSpeed * Time.deltaTime);

        // Look at the target
        transform.LookAt(target);
    }
}