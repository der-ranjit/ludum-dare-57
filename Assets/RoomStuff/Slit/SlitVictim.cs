using UnityEngine;

public class SlitVictim : MonoBehaviour
{
    private bool isTriggered = false;
    private float duration = 1.0f; // Duration for moving downward
    private float elapsedTime = 0.0f;
    private float downwardSpeed = 1.5f; // Speed of downward movement

    public void Trigger()
    {
        if (isTriggered) return;

        Debug.Log("SlitVictim Triggered!");
        isTriggered = true;

        // Disable physics interactions
        if (TryGetComponent<Rigidbody>(out Rigidbody rb))
        {
            rb.isKinematic = true;
        }
        if (TryGetComponent<Collider>(out Collider col))
        {
            col.enabled = false;
        }
    }

    private void Update()
    {
        if (isTriggered)
        {
            elapsedTime += Time.deltaTime;

            // Move the object downward
            transform.position += Vector3.down * downwardSpeed * Time.deltaTime;

            // Destroy the object after the duration
            if (elapsedTime >= duration)
            {
                Destroy(gameObject);
            }
        }
    }
}