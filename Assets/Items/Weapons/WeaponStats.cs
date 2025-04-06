using UnityEngine;

[CreateAssetMenu(fileName = "NewWeapon", menuName = "Weapons/Weapon")]
public class WeaponStats : ScriptableObject
{
    public enum WeaponType
    {
        Ranged,
        Melee
    }

    public string weaponName;
    public GameObject weaponPrefab;
    public WeaponType weaponType; // Ranged or Melee
    public float damage = 1.0f;
    public float fireRate = 1.0f;
    public int piercing = 0;
    public float size = 1.0f;
    public float attackAngle = 0.0f;
    public float pushForce = 0.0f;

    // Ranged-specific fields
    [Header("Ranged Weapon Settings")]
    public GameObject bulletPrefab; // Prefab for bullets (only for ranged weapons)
    public float bulletLifetime = 5f; // Lifetime of the bullet before it disappears
    public float bulletSpeed;
    public float bulletRange;
    public int bulletBounce;
}