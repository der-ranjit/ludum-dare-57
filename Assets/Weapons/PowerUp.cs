using System.Data.Common;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    public WeaponStats powerUpStats; // The stats this power-up modifies

    void Start()
    {
        // when no powerup stats are defined, generate random ones
        if (powerUpStats == null)
        {
            GenerateRandomPowerUp();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the player collects the power-up
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                // Apply the power-up to the player's stats
                player.ApplyPowerUp(powerUpStats);
                string[] texts = new string[] { "something is better now?", "more stronger?", "more faster?", "more damage?" };
                string randomString = texts[UnityEngine.Random.Range(0, texts.Length)];
                TextParticleSystem.ShowEffect(transform.position + Vector3.up * 0.1f, randomString);

                // Destroy the power-up object
                Destroy(gameObject);
            }
        }
    }

    // Method to randomly define a power-up
    public void GenerateRandomPowerUp()
    {
        powerUpStats = ScriptableObject.CreateInstance<WeaponStats>();
        powerUpStats.weaponName = "Random Power-Up";
        powerUpStats.damage = Random.Range(1.05f, 1.1f);
        powerUpStats.bulletSpeed = Random.Range(1.05f, 1.1f);
        powerUpStats.size = Random.Range(1.02f, 1.05f);
        if (Random.value > 0.9f)
        {
            powerUpStats.piercing = 1;
        }
        if (Random.value > 0.9f)
        {
            powerUpStats.bulletBounce = 1;
        }
    }
}