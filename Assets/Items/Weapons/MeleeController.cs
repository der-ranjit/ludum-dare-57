using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class MeleeController : MonoBehaviour
{
    private float damage;
    private float attackAngle;
    private BoxCollider attackCollider;

    public void Initialize(WeaponStats stats)
    {
        // Set damage and attack angle
        damage = stats.damage;
        attackAngle = stats.attackAngle;

        // angle and scale the weapon prefab's transform
        stats.weaponPrefab.transform.localScale *= stats.size;
        stats.weaponPrefab.transform.localRotation = Quaternion.Euler(0, attackAngle, 0);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the target implements the IDamageable interface
        if (other.TryGetComponent<IDamageable>(out IDamageable damageable))
        {
            // Apply damage to the target
            damageable.TakeDamage(damage);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (attackCollider != null)
        {
            // Visualize the BoxCollider in the Scene view
            Gizmos.color = Color.red;
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawWireCube(attackCollider.center, attackCollider.size);
        }
    }
}