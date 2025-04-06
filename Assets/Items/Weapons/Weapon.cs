using System.Collections;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public WeaponStats baseStats;
    private float nextFireTime = 0;
    private WeaponStats upgradedStats;
    private GameObject weaponHolder;

    GameObject attachedWeaponPrefab; // The weapon prefab attached to the player

    public void Start()
    {
        upgradedStats = baseStats; 
        Transform weaponHolderTransform = transform.Find("WeaponHolder");
        if (weaponHolderTransform != null)
        {   
            weaponHolder = weaponHolderTransform.gameObject;
            Debug.LogWarning("WeaponHolder not found. Weapon not attached.");
        }

        AttachToCharacter();
    }

    public void Initialize(PlayerStats playerStats)
    {
        // Precompute adjusted stats based on player stats
        upgradedStats = playerStats.ApplyPowerUps(new WeaponStats
        {
            weaponName = baseStats.weaponName,
            weaponType = baseStats.weaponType,
            weaponPrefab = baseStats.weaponPrefab,
            damage = baseStats.damage,
            fireRate = baseStats.fireRate,
            piercing = baseStats.piercing,
            size = baseStats.size,
            attackAngle = baseStats.attackAngle,
            pushForce = baseStats.pushForce,
            bulletLifetime = baseStats.bulletLifetime,
            bulletSpeed = baseStats.bulletSpeed,
            bulletRange = baseStats.bulletRange,
            bulletBounce = baseStats.bulletBounce,
            bulletPrefab = baseStats.bulletPrefab
        });
    }

    // Attach the weapon the the WeaponHolder of the character
    public void AttachToCharacter()
    {
        if (weaponHolder != null)
        {
            // instantiate the weapon prefab and set it as a child of the weapon holder
            if (upgradedStats.weaponPrefab != null)
            {
                attachedWeaponPrefab = Instantiate(upgradedStats.weaponPrefab, weaponHolder.transform);

                // ignore collisions with enemies if the weapon holder is an enemy
                if (gameObject.CompareTag("Enemy"))
                {
                    Collider[] colliders = attachedWeaponPrefab.GetComponentsInChildren<Collider>();
                    foreach (Collider collider in colliders)
                    {
                        // cool, just look it up in the settings brah
                        collider.excludeLayers = 1 << LayerMask.NameToLayer("Enemies");
                    }
                }
            }
        }
        else
        {
            Debug.LogWarning("WeaponHolder not found. Weapon not attached.");
        }
    }

    public void Attack(GameObject target)
    {
        if (upgradedStats.weaponType == WeaponStats.WeaponType.Ranged)
        {
            FireRangedWeapon(target);
        }
        else if (upgradedStats.weaponType == WeaponStats.WeaponType.Melee)
        {
            PerformMeleeAttack(target);
        }
    }

    public void FireRangedWeapon(GameObject target)
    {
        if (Time.time >= nextFireTime)
        {
            Debug.Log("Firing ranged weapon at target: " + target.name);
            nextFireTime = Time.time + 1f / upgradedStats.fireRate;

            Transform firePoint = weaponHolder?.transform.Find("FirePoint");
            // If no fire point is set, use the weapon holder's position
            if (firePoint == null)
            {
                firePoint = weaponHolder?.transform;
            }
            if (firePoint == null)
            {
                firePoint = gameObject.transform;
            }

            // Calculate the direction to the player
            Vector3 directionToTarget = (target.transform.position - transform.position).normalized;

            // Instantiate the bullet
            GameObject bullet = Instantiate(upgradedStats.bulletPrefab, firePoint.position, Quaternion.LookRotation(directionToTarget));
            RangedController bulletComponent = bullet.GetComponent<RangedController>();
            bulletComponent.Initialize(upgradedStats);
            // set all bullets colliders exclude layers to exclude the enemies layer when the weapon is attached to an character with tag enemy
            if (gameObject.CompareTag("Enemy"))
            {
                Collider[] colliders = bullet.GetComponentsInChildren<Collider>();
                foreach (Collider collider in colliders)
                {
                    collider.excludeLayers = 1 << LayerMask.NameToLayer("Enemies");
                }
            }
        }
    }

    public void PerformMeleeAttack(GameObject target)
    {
        // For now just stab the weapon prefab in the forward quickly and return to its previous position
        // Get the MeleeController component from the weapon prefab
        MeleeController meleeController = attachedWeaponPrefab.GetComponent<MeleeController>();
        // Perform the melee attack using the MeleeController
        meleeController.Initialize(upgradedStats);

        // Simulate a quick stab motion
        StartCoroutine(StabMotion());
    }

    private IEnumerator StabMotion()
    {
        if (attachedWeaponPrefab == null)
        {
            yield break;
        }

        // Save the original position and rotation of the weapon
        Vector3 originalPosition = attachedWeaponPrefab.transform.localPosition;
        Quaternion originalRotation = attachedWeaponPrefab.transform.localRotation;

        // Define the stab position and rotation
        Vector3 stabPosition = originalPosition + Vector3.forward * 0.5f; // Move forward slightly
        Quaternion stabRotation = Quaternion.Euler(originalRotation.eulerAngles + new Vector3(30f, 0f, 0f)); // Add a slight tilt

        // Move to the stab position
        float stabDuration = 0.1f; // Duration of the stab motion
        float elapsedTime = 0f;
        while (elapsedTime < stabDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / stabDuration;

            attachedWeaponPrefab.transform.localPosition = Vector3.Lerp(originalPosition, stabPosition, t);
            attachedWeaponPrefab.transform.localRotation = Quaternion.Slerp(originalRotation, stabRotation, t);

            yield return null;
        }

        // Return to the original position
        elapsedTime = 0f;
        while (elapsedTime < stabDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / stabDuration;

            attachedWeaponPrefab.transform.localPosition = Vector3.Lerp(stabPosition, originalPosition, t);
            attachedWeaponPrefab.transform.localRotation = Quaternion.Slerp(stabRotation, originalRotation, t);

            yield return null;
        }

        // Ensure the weapon returns to its exact original position and rotation
        attachedWeaponPrefab.transform.localPosition = originalPosition;
        attachedWeaponPrefab.transform.localRotation = originalRotation;
    }
}