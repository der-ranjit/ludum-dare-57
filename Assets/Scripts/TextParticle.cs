using System.Collections;
using UnityEngine;
using TMPro;

public class TextParticle : MonoBehaviour
{
    private TextMeshPro textMesh;
    private float lifetime = 2.0f;
    private float fadeOutDuration = 0.5f;
    private Vector3 moveDirection = Vector3.up;
    private float moveSpeed = 0.5f;

    private float age = 0.0f;
    private float rotationFactor;
    
    public static TextParticle Create(Vector3 position, string text, float lifetime = 2.0f, Color? color = null, float size = 1.0f)
    {
        // Create the game object with TextParticle component
        GameObject go = new GameObject("TextParticle");
        TextParticle textParticle = go.AddComponent<TextParticle>();
        
        // Add TextMeshPro component
        TextMeshPro tmp = go.AddComponent<TextMeshPro>();
        tmp.text = text;
        tmp.fontSize = 5 * size;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = color ?? Color.white;
        
        // Make it face the camera
        go.transform.position = position;
        
        // Initialize the component
        textParticle.Init(tmp, lifetime);
        
        return textParticle;
    }
    
    private void Init(TextMeshPro tmp, float lifetime)
    {
        this.textMesh = tmp;
        this.lifetime = lifetime;
        this.rotationFactor = Random.Range(-60f, 60f); // Random rotation factor for the text particle
        
        // Face the camera
        FaceCamera();
        
        // Start lifetime and fade coroutine
        StartCoroutine(LifetimeRoutine());
    }
    
    private IEnumerator LifetimeRoutine()
    {
        float elapsed = 0;
        float lifetimeBeforeFade = lifetime - fadeOutDuration;
        
        // Wait until it's time to fade
        while (elapsed < lifetimeBeforeFade)
        {
            elapsed += Time.deltaTime;
            transform.position += moveDirection * moveSpeed * Time.deltaTime;
            yield return null;
        }
        
        // Fade out
        float alpha;
        Color color = textMesh.color;
        
        while (elapsed < lifetime)
        {
            elapsed += Time.deltaTime;
            alpha = 1.0f - (elapsed - lifetimeBeforeFade) / fadeOutDuration;
            color.a = alpha;
            textMesh.color = color;
            transform.position += moveDirection * moveSpeed * Time.deltaTime;
            yield return null;
        }
        
        // Destroy when done
        Destroy(gameObject);
    }
    
    private void Update()
    {
        age += Time.deltaTime;
        // Rotate the text particle to face the camera
        FaceCamera();
        // Rotate around local x axis
        float rotateBy = age * rotationFactor;
        transform.Rotate(0, 0, rotateBy);
    }
    
    private void FaceCamera()
    {
        if (Camera.main != null)
        {
            transform.rotation = Camera.main.transform.rotation;
        }
    }
}