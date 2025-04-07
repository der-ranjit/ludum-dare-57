using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
[RequireComponent(typeof(SpriteRenderer))]
public class SpriteSetupHelper : MonoBehaviour
{
    private void Start()
    {
#if UNITY_EDITOR
        // Ensure the setup runs in the editor
        if (!Application.isPlaying)
        {
            SetupSprite();
        }
#endif
    }

#if UNITY_EDITOR
    private void SetupSprite()
    {
        // Get the SpriteRenderer component
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError("SpriteRenderer not found on the GameObject.");
            return;
        }

        Material spriteShaderMaterial = Resources.Load<Material>("SpriteShader");
        if (spriteShaderMaterial != null)
        {
            spriteRenderer.material = spriteShaderMaterial;
            Debug.Log("Premade SpriteShader material applied to the sprite.");
        }
        else
        {
            Debug.LogWarning("Custom shader 'SpriteShader' not found.");
        }


        // Enable shadows
        Renderer renderer = spriteRenderer.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
            renderer.receiveShadows = true;
        }
        else
        {
            Debug.LogError("Renderer component not found on the GameObject.");
        }

        DestroyImmediate(this);
    }
#endif
}