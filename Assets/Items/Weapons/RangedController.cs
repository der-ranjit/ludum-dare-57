using UnityEngine;

public class RangedController : MonoBehaviour
{
    private float lifetime = 5f;

    private float damage;
    private float speed;
    private int piercing;
    private int bounce;
    private float pushForce;
    public GameObject shooter;

    public void Initialize(WeaponStats stats)
    {
        damage = stats.damage;
        speed = stats.bulletSpeed;
        piercing = stats.piercing;
        bounce = stats.bulletBounce;
        lifetime = stats.bulletLifetime;
        pushForce = stats.pushForce;

        // Adjust bullet size
        transform.localScale *= stats.size;
        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        // Move the bullet forward
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    private void OnCollisionEnter(Collision other)
    {
        if(other.gameObject == shooter)
        {
            return; // Ignore collision with the shooter
        }
        if (other.gameObject.TryGetComponent<IDamageable>(out IDamageable damageable))
        {
            damageable.TakeDamage(damage);

            // Apply push force 
            Rigidbody playerRb = other.gameObject.GetComponent<Rigidbody>();
            if (playerRb != null)
            {
                Vector3 pushDirection = (other.transform.position - transform.position).normalized;
                playerRb.AddForce(pushDirection * pushForce, ForceMode.Impulse);
            }

            // Handle piercing
            piercing--;
            if (piercing <= 0)
            {
                Destroy(gameObject);
            }
        }
                // Handle bouncing (optional)
        if (bounce > 0)
        {
            // Implement bouncing logic here
            bounce--;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}