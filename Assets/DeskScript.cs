using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeskScript : MonoBehaviour
{
    private float nextParticle = 0f;

    
    string[] typingTexts = new string[] { "Tip", "Tap", "Clack", "Click", "Tock", "Tick", "Tap", "Tap", "Tap" };

    private bool isAnimating = true;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isAnimating && Time.time > nextParticle)
        {
            nextParticle = Time.time + Random.Range(0.2f, 0.3f);
            string typingText = typingTexts[Random.Range(0, typingTexts.Length)];
            TextParticleSystem.ShowTinyEffect(transform.position + new Vector3(Random.Range(-0.15f, 0.15f), 0.01f, Random.Range(-0.3f, 0.3f)), typingText);
        }
    }

    public void EnableAnimation() {
        isAnimating = true;
    }

    public void DisableAnimation() {
        isAnimating = false;
    }
}
