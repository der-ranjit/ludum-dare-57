// Add this to your GameManager or create a new FadeManager class
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScreenFader : MonoBehaviour
{
    [SerializeField] private Image fadeImage;
    [SerializeField] private float defaultFadeDuration = 1.0f;
    
    public static ScreenFader Instance { get; private set; }
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        // Make sure the image starts fully transparent
        fadeImage.color = new Color(0, 0, 0, 0);
        // Set image width to cover full screen
        RectTransform rectTransform = fadeImage.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(Screen.width, Screen.height);
    }
    
    public void FadeToBlack(float duration = -1)
    {
        StartCoroutine(FadeRoutine(0, 1, duration > 0 ? duration : defaultFadeDuration));
    }
    
    public void FadeFromBlack(float duration = -1)
    {
        StartCoroutine(FadeRoutine(1, 0, duration > 0 ? duration : defaultFadeDuration));
    }
    
    public IEnumerator FadeRoutine(float startAlpha, float targetAlpha, float duration)
    {
        float elapsedTime = 0;
        Color currentColor = fadeImage.color;
        
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float newAlpha = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / duration);
            fadeImage.color = new Color(currentColor.r, currentColor.g, currentColor.b, newAlpha);
            yield return null;
        }
        
        // Ensure we end at exactly the target alpha
        fadeImage.color = new Color(currentColor.r, currentColor.g, currentColor.b, targetAlpha);
    }

    public void setCurrentFadeAlpha(float alpha)
    {
        Color currentColor = fadeImage.color;
        fadeImage.color = new Color(currentColor.r, currentColor.g, currentColor.b, alpha);
    }
}