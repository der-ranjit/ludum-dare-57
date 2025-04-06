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

            }
        }
        else
        {
            Debug.LogWarning("WeaponHolder not found. Weapon not attached.");
        }
    }

    public void Attack(Vector3 direction)
    {
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
        if (Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + 1f / upgradedStats.fireRate;

            Transform firePoint = weaponHolder?.transform.Find("FirePoint");
            // If no fire point is set, use the weapon holder's position
            if (firePoint == null)
            {
                firePoint = weaponHolder?.transform;
            }
            if (firePoint == null)
            {
                firePoint = transform.parent;
            }

            // Instantiate the bullet
            GameObject bullet = Instantiate(upgradedStats.bulletPrefab, firePoint.position, Quaternion.LookRotation(direction));
            RangedController bulletComponent = bullet.GetComponent<RangedController>();
            bulletComponent.shooter = gameObject; // Set the shooter to the current weapon
            bulletComponent.Initialize(upgradedStats);
        }
    }

    public void PerformMeleeAttack(Vector3 direction)
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