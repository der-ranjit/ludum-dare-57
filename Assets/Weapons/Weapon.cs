using System.Collections;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public WeaponStats baseStats;
    private float nextFireTime = 0;
    private WeaponStats upgradedStats;
    private GameObject weaponHolder;

    GameObject attachedWeaponPrefab; // The weapon prefab attached to the player
    public float swingDuration = 0.5f;
    public float forwardOffset = 0.1f;
    public float swingRadius = 0.5f;
    private bool isSwinging = false;

    public void Start()
    {
        upgradedStats = baseStats;
        Transform weaponHolderTransform = gameObject.transform.Find("WeaponHolder");
        if (weaponHolderTransform != null)
        {
            weaponHolder = weaponHolderTransform.gameObject;
            Debug.LogWarning("WeaponHolder not found. Weapon not attached.");
        }

        AttachToCharacter();
    }

    public void ApplyPlayerPowerUpStats(WeaponStats playerPowerUpStats)
    {
        // Precompute adjusted stats based on player stats
        upgradedStats = ScriptableObject.CreateInstance<WeaponStats>();
        upgradedStats.damage = baseStats.damage * playerPowerUpStats.damage;
        upgradedStats.bulletSpeed = baseStats.bulletSpeed * playerPowerUpStats.bulletSpeed;
        upgradedStats.bulletLifetime = baseStats.bulletLifetime * playerPowerUpStats.bulletLifetime;
        upgradedStats.bulletRange = baseStats.bulletRange * playerPowerUpStats.bulletRange;
        upgradedStats.pushForce = baseStats.pushForce * playerPowerUpStats.pushForce;
        upgradedStats.fireRate = baseStats.fireRate * playerPowerUpStats.fireRate;
        upgradedStats.size = baseStats.size * playerPowerUpStats.size;
        upgradedStats.piercing = baseStats.piercing + playerPowerUpStats.piercing;
        upgradedStats.bulletBounce = baseStats.bulletBounce + playerPowerUpStats.bulletBounce;
        // Copy other stats from baseStats
        upgradedStats.bulletPrefab = baseStats.bulletPrefab;
        upgradedStats.weaponName = baseStats.weaponName;
        upgradedStats.weaponType = baseStats.weaponType;
        upgradedStats.attackAngle = baseStats.attackAngle;
        upgradedStats.weaponPrefab = baseStats.weaponPrefab;
        upgradedStats.bulletGravity = baseStats.bulletGravity;
    }

    // Attach the weapon the the WeaponHolder of the character
    public void AttachToCharacter()
    {
        if (weaponHolder != null)
        {
            // instantiate the weapon prefab and set it as a child of the weapon holder
            if (upgradedStats.weaponPrefab != null)
            {
                // Destroy the previous weapon prefab if it exists
                if (attachedWeaponPrefab != null)
                {
                    Destroy(attachedWeaponPrefab);
                }
                attachedWeaponPrefab = Instantiate(upgradedStats.weaponPrefab, weaponHolder.transform);
                // ignore collision between the weapon and the character
                Collider[] characterColliders = GetComponents<Collider>();
                BoxCollider weaponCollider = attachedWeaponPrefab.GetComponent<BoxCollider>();
                foreach (Collider characterCollider in characterColliders)
                {
                    // Ignore collision between the weapon and the character's colliders
                    if (characterCollider != null && weaponCollider != null && characterCollider != weaponCollider)
                    {
                        Physics.IgnoreCollision(characterCollider, weaponCollider);
                    }
                }
            }
        }
        else
        {
            Debug.LogWarning("WeaponHolder not found. Weapon not attached.");
        }
    }

    public void Attack(Vector3 direction)
    {
        if (Time.time < nextFireTime) return;
        float randomAttackDelay = gameObject.CompareTag("Enemy") ? Random.Range(0f, 0.5f) : 0;
        nextFireTime = Time.time + (1f / upgradedStats.fireRate) + randomAttackDelay;
        if (upgradedStats.weaponType == WeaponStats.WeaponType.Ranged)
        {
            FireRangedWeapon(direction);
        }
        else if (upgradedStats.weaponType == WeaponStats.WeaponType.Melee)
        {
            PerformMeleeAttack(direction);
        }
    }

    public void FireRangedWeapon(Vector3 direction)
    {
        Transform firePoint = attachedWeaponPrefab?.transform.Find("FirePoint");
        // If no fire point is set, use the weapon holder's position
        if (firePoint == null)
        {
            firePoint = weaponHolder?.transform;
        }
        if (firePoint == null)
        {
            firePoint = transform;
        }
        // Instantiate the bullet
        GameObject bullet = Instantiate(upgradedStats.bulletPrefab, firePoint.position, Quaternion.LookRotation(direction));
        RangedController bulletComponent = bullet.GetComponent<RangedController>();
        bulletComponent.shooter = gameObject; // Set the shooter to the current weapon
        bulletComponent.Initialize(upgradedStats);
    }

    public void PerformMeleeAttack(Vector3 direction)
    {
        // For now just stab the weapon prefab in the forward quickly and return to its previous position
        // Get the MeleeController component from the weapon prefab
        MeleeController meleeController = attachedWeaponPrefab.GetComponent<MeleeController>();
        // Perform the melee attack using the MeleeController
        meleeController.Initialize(upgradedStats);
        if (attachedWeaponPrefab == null || isSwinging)
        {
            return;
        }
        // Perform a random swing
        StartCoroutine(SwingSword());
    }

    private IEnumerator SwingSword()
    {
        isSwinging = true;

        Transform swordTransform = attachedWeaponPrefab.transform;
        Transform playerTransform = weaponHolder.transform; // Assuming the weapon holder is the parent of the sword
        // Step 1: Set sword orientation (local X-axis 90 degrees to lay horizontal)
        Quaternion initialRotation = swordTransform.localRotation;
        Vector3 initialPosition = swordTransform.position;
        swordTransform.localRotation = Quaternion.Euler(90f, 0f, 0f);

        // Step 2: Calculate initial swing position
        Vector3 forwardOffsetVec = playerTransform.forward * forwardOffset;
        Vector3 startDirection = Quaternion.AngleAxis(-90f, playerTransform.up) * forwardOffsetVec.normalized;
        Vector3 startPosition = playerTransform.position + startDirection * swingRadius;

        swordTransform.position = startPosition;

        // Step 3: Animate the swing over time
        float elapsed = 0f;
        while (elapsed < swingDuration)
        {
            if (playerTransform == null)
            {
                yield break; // Exit if the player or sword transform is not found
            }
            float t = elapsed / swingDuration;
            float angle = Mathf.Lerp(90f, -90f, t); // From -90 to +90 degrees
            Vector3 swingDir = Quaternion.AngleAxis(angle, playerTransform.up) * forwardOffsetVec.normalized;
            swordTransform.position = playerTransform.position + swingDir * swingRadius;

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Step 4: Reset the sword to its original position and rotation
        swordTransform.localRotation = initialRotation;
        swordTransform.position = initialPosition;

        // End of swing
        isSwinging = false;
    }

    public void Show()
    {
        if (weaponHolder != null)
        {
            SpriteRenderer weaponSprite = weaponHolder.GetComponentInChildren<SpriteRenderer>();
            if (weaponSprite != null)
            {
                weaponSprite.enabled = true; // Show the weapon sprite
            }
        }
    }

    public void Hide()
    {
        if (weaponHolder != null)
        {
            SpriteRenderer weaponSprite = weaponHolder.GetComponentInChildren<SpriteRenderer>();
            if (weaponSprite != null)
            {
                weaponSprite.enabled = false; // Show the weapon sprite
            }
        }
    }
}