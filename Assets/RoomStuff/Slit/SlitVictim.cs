using UnityEngine;

public class SlitVictim : MonoBehaviour
{
    private bool isTriggered = false;
    private float duration = 1.0f; // Duration for moving downward
    private float elapsedTime = 0.0f;
    private float downwardSpeed = 1.5f; // Speed of downward movement

    string[] slitVictimTexts = new string[] { "Ouch!", "Nooooo!", "*Splatter*", "Help!", "Why me?", "Aaaahhh!", "Oh-oh!" };


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

        // Trigger text particle
        string randomString = slitVictimTexts[UnityEngine.Random.Range(0, slitVictimTexts.Length)];
        TextParticleSystem.ShowDamage(transform.position + Vector3.up * 0.1f, randomString);
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