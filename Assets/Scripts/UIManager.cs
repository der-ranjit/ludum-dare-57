using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    public Text enemyCountText;
    public Text playerHealthText;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void UpdateEnemyCount(int killed, int total)
    {
        enemyCountText.text = $"Enemies: {killed}/{total}";
    }

    public void UpdatePlayerHealth(float currentHealth, float maxHealth)
    {
        playerHealthText.text = $"Health: {currentHealth}/{maxHealth}";
    }
}