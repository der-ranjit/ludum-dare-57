using UnityEngine;

[System.Serializable]
public class PlayerStats
{
    public float maxHealth = 1f;
    public float currentHealth = 1f;

    public float powerUpBulletSize = 1;
    public float powerUpBulletSpeed = 1;
    public float powerUpBulletDamage = 1;
    public float powerUpBulletPiercing = 1;
    public float powerUpBulletBounce = 1;

    // Method to apply power-ups to weapon stats
    public WeaponStats ApplyPowerUps(WeaponStats baseStats)
    {
        return new WeaponStats
        {
            damage = baseStats.damage * powerUpBulletDamage,
            bulletSpeed = baseStats.bulletSpeed * powerUpBulletSpeed,
            size = baseStats.size * powerUpBulletSize,
            piercing = Mathf.FloorToInt(baseStats.piercing * powerUpBulletPiercing),
            bulletBounce = Mathf.FloorToInt(baseStats.bulletBounce * powerUpBulletBounce)
        };
    }
}