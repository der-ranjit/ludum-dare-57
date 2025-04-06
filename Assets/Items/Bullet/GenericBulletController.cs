using UnityEngine;

public class GenericBulletController : MonoBehaviour
{
    public float speed = 10f; // Speed of the bullet
    public float pushForce = 5f; // Force to push the player on collision
    public float lifetime = 5f; // Lifetime of the bullet before it disappears

    private void Start()
    {
        // Destroy the bullet after its lifetime
        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        // Move the bullet forward
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Apply a push force to the player
            Rigidbody playerRb = other.GetComponent<Rigidbody>();
            if (playerRb != null)
            {
                Vector3 pushDirection = (other.transform.position - transform.position).normalized;
                playerRb.AddForce(pushDirection * pushForce, ForceMode.Impulse);
            }

            // Destroy the bullet
            Destroy(gameObject);
        }
    }
}