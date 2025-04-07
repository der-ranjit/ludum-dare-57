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
    private Rigidbody bulletRigidBody;
    Vector3 shootDirection;

    public void Initialize(WeaponStats stats)
    {
        IgnoreFriendlyFire(shooter);
        IgnoreCollisionWithShooter(shooter);

        damage = stats.damage;
        speed = stats.bulletSpeed;
        piercing = stats.piercing;
        bounce = stats.bulletBounce;
        lifetime = stats.bulletLifetime;
        pushForce = stats.pushForce;

        // Adjust bullet size
        transform.localScale *= stats.size;

        // shoot the bullet forward, but upwards in repsect to the stats attackAngle
        shootDirection = Quaternion.Euler(-stats.attackAngle, 0, 0) * transform.forward;

        bulletRigidBody = GetComponent<Rigidbody>();
        if (bulletRigidBody != null)
        {
            if (stats.bulletGravity)
            {
                bulletRigidBody.useGravity = true;
                bulletRigidBody.isKinematic = false;
            }
            else
            {
                bulletRigidBody.useGravity = false;
                bulletRigidBody.isKinematic = true;
            }
        }
        if (stats.bulletGravity)
        {
            bulletRigidBody.velocity = shootDirection * speed;
        }
        Destroy(gameObject, lifetime);
    }

    private void Update()
    {
        if (bulletRigidBody != null && bulletRigidBody.isKinematic)
        {
            // Move the bullet forward
            transform.position += shootDirection * speed * Time.deltaTime;
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        Collider[] shooterCollider = shooter.GetComponentsInChildren<Collider>();
        bool isFriendlyFireBetweenEnemies = shooter.CompareTag("Enemy") && other.gameObject.CompareTag("Enemy");
        if (isFriendlyFireBetweenEnemies)
        {
            return;
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

    private void IgnoreCollisionWithShooter(GameObject shooter)
    {
        Collider[] shooterCollider = shooter.GetComponentsInChildren<Collider>();
        Collider bulletCollider = GetComponent<Collider>();
        if (bulletCollider != null)
        {
            foreach (Collider collider in shooterCollider)
            {
                Physics.IgnoreCollision(bulletCollider, collider);
            }
        }
    }

    private void IgnoreFriendlyFire(GameObject shooter)
    {
        if (shooter.CompareTag("Enemy"))
        {
            // get all enemies in the scene
            Enemy[] enemies = FindObjectsOfType<Enemy>();
            Collider bulletCollider = GetComponent<Collider>();

            foreach (Enemy enemy in enemies)
            {
                foreach (Collider enemyCollider in enemy.GetComponents<Collider>())
                {
                    Physics.IgnoreCollision(bulletCollider, enemyCollider);
                }
            }
        }
    }
}