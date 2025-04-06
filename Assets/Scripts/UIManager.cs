using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    public Text enemyCountText;
    public Text playerHealthText;

    private void Start()
    {
        enemyCountText = gameObject.AddComponent<Text>();
        enemyCountText.fontSize = 24;
        enemyCountText.color = Color.white;
        enemyCountText.alignment = TextAnchor.UpperLeft;
        enemyCountText.rectTransform.anchoredPosition = new Vector2(10, -10);
        enemyCountText.rectTransform.SetParent(transform, false);
        enemyCountText.text = "Enemies: 0/0";
        enemyCountText.gameObject.SetActive(true);

        playerHealthText = gameObject.AddComponent<Text>();
        playerHealthText.fontSize = 24;
        playerHealthText.color = Color.white;
        playerHealthText.alignment = TextAnchor.UpperRight;
        playerHealthText.rectTransform.anchoredPosition = new Vector2(-10, -10);
        playerHealthText.rectTransform.SetParent(transform, false);
        playerHealthText.text = "Health: 0/0";
        playerHealthText.gameObject.SetActive(true);
    }

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