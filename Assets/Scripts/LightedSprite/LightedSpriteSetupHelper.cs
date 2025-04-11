using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(SpriteRenderer))]
public class LightedSpriteSetupHelper : MonoBehaviour
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
    // 
    private void SetupSprite()
    {
        SpriteSyncer syncer = GetComponentInChildren<SpriteSyncer>();
        if (syncer != null)
        {
            return;
        }

        // Get the SpriteRenderer component
        SpriteRenderer original = GetComponent<SpriteRenderer>();
        if (original == null)
        {
            Debug.LogError("SpriteRenderer not found on the GameObject.");
            return;
        }

        Material spriteShaderMaterial = Resources.Load<Material>("SpriteShader");
        if (spriteShaderMaterial == null)
        {
            Debug.LogWarning("Custom shader 'SpriteShader' not found.");
        }
        original.material = spriteShaderMaterial;

        // Create a second sprite to emulate two faced lighting on the sprite.
        // mirror sprite and push it in front of the original sprite.
        GameObject cloneObj = new GameObject("MirroredSprite");
        cloneObj.transform.parent = transform;
        cloneObj.transform.localRotation = original.transform.localRotation;
        cloneObj.transform.localPosition = new Vector3(0, 0, +0.001f);
        // flip original sprite
        cloneObj.transform.Rotate(0, 180, 0);
        cloneObj.transform.localScale = new Vector3(-1, 1, 1);

        SpriteRenderer clone = cloneObj.AddComponent<SpriteRenderer>();
        clone.sprite = original.sprite;
        clone.material = spriteShaderMaterial;
        clone.sortingLayerID = original.sortingLayerID;
        clone.sortingOrder = original.sortingOrder;
        clone.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        clone.receiveShadows = true;
        clone.gameObject.AddComponent<SpriteSyncer>();
        
        original.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
        original.receiveShadows = true;


        DestroyImmediate(this);
    }
#endif
}