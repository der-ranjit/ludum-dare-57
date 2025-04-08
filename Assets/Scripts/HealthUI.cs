using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class HealthUI : MonoBehaviour
{
    [SerializeField] private Image heartPrefab;
    [SerializeField] private float heartSpacing = 25f;
    [SerializeField] private int initialHitpoints = 0;
    [SerializeField] private Vector2 topRightOffset = new Vector2(-10, -0);
    
    private int currentHitpoints = 0;
    private List<Image> heartImages = new List<Image>();
    private RectTransform heartsContainer;
    
    private void Start()
    {
        if (heartPrefab == null)
        {
            Debug.LogError("Heart prefab not assigned to HealthUI!");
            return;
        }
        
        SetupContainer();
        SetHitpoints(initialHitpoints);
    }
    
    private void SetupContainer()
    {
        // Create a container for hearts
        GameObject container = new GameObject("HeartsContainer");
        container.transform.SetParent(transform, false);
        heartsContainer = container.AddComponent<RectTransform>();

        // Set as first child
        container.transform.SetSiblingIndex(0);
            
        // Position in top right corner
        heartsContainer.anchorMin = new Vector2(1, 1);
        heartsContainer.anchorMax = new Vector2(1, 1);
        heartsContainer.pivot = new Vector2(1, 1);
        heartsContainer.anchoredPosition = topRightOffset;
    }
    
    public void SetHitpoints(int hitpoints)
    {
        currentHitpoints = Mathf.Max(0, hitpoints); // Ensure hitpoints don't go negative
        
        // Add hearts if we need more
        while (heartImages.Count < currentHitpoints)
        {
            AddHeart();
        }
        
        // Update heart visibility
        for (int i = 0; i < heartImages.Count; i++)
        {
            heartImages[i].gameObject.SetActive(i < currentHitpoints);
            // Scale heart icon x8
            heartImages[i].rectTransform.localScale = new Vector3(8, 8, 1);
        }
    }
    
    private void AddHeart()
    {
        Image newHeart = Instantiate(heartPrefab, heartsContainer);
        RectTransform heartRect = newHeart.GetComponent<RectTransform>();
        
        // Set proper anchoring for positioning
        heartRect.anchorMin = new Vector2(1, 0.5f);
        heartRect.anchorMax = new Vector2(1, 0.5f);
        heartRect.pivot = new Vector2(1, 0.5f);
        
        // Position the heart
        float heartWidth = heartPrefab.rectTransform.rect.width;
        float xPosition = -(heartWidth + heartSpacing) * heartImages.Count;
        heartRect.anchoredPosition = new Vector2(xPosition, 0);
        
        heartImages.Add(newHeart);
    }
}