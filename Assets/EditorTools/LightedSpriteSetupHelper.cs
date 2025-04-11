#if UNITY_EDITOR
    using UnityEditor;
#endif
using UnityEngine;

[ExecuteInEditMode]
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
    [MenuItem("GameObject/Lighted Sprite Setup Helper", false, 0)]
    private static void AddLightedSpriteSetupHelper(MenuCommand menuCommand)
    {
        // Get the selected GameObject
        GameObject selectedObject = menuCommand.context as GameObject;

        if (selectedObject != null)
        {
            // Add the LightedSpriteSetupHelper component to the selected GameObject
            selectedObject.AddComponent<LightedSpriteSetupHelper>();
            Debug.Log("LightedSpriteSetupHelper added to " + selectedObject.name);
        }
        else
        {
            Debug.LogError("No GameObject selected to add LightedSpriteSetupHelper.");
        }
    }
    // 
    private void SetupSprite()
    {
        SpriteSyncer syncer = GetComponentInChildren<SpriteSyncer>();
        if (syncer != null)
        {
            DestroyImmediate(this);
            return;
        }

        CameraFacer cameraFacer = GetComponentInParent<CameraFacer>();
        if (cameraFacer != null)
        {
            Debug.LogWarning("CameraFacer does not make sense with LightedSprites. Skipping setup.");
            DestroyImmediate(this);
            return;
        }

        // Check if the current object has a SpriteRenderer
        SpriteRenderer original = GetComponent<SpriteRenderer>();

        // If no SpriteRenderer exists on the current object, search for a child named "Sprite"
        if (original == null)
        {
            Transform spriteTransform = transform.Find("Sprite");
            original = spriteTransform != null ? spriteTransform.GetComponent<SpriteRenderer>() : null;

            // If no "Sprite" GameObject exists, create a new child with a SpriteRenderer
            if (spriteTransform == null && original == null)
            {
                spriteTransform = new GameObject("Sprite").transform;
                spriteTransform.parent = transform;
                original = spriteTransform.gameObject.AddComponent<SpriteRenderer>();
            }
        }

        // If no SpriteRenderer exists at all, log an error and stop
        if (original == null)
        {
            Debug.LogError("SpriteRenderer not found on the GameObject.");
            DestroyImmediate(this);
            return;
        }

        Material spriteShaderMaterial = Resources.Load<Material>("SpriteShader");
        if (spriteShaderMaterial == null)
        {
            Debug.LogWarning("Custom shader 'SpriteShader' not found.");
            DestroyImmediate(this);
            return;
        }
        original.material = spriteShaderMaterial;

        // Create a second sprite to emulate two faced lighting on the sprite.
        // mirror sprite and push it in front of the original sprite.
        GameObject cloneObj = new GameObject("MirroredSprite");
        cloneObj.transform.parent = original.transform;
        cloneObj.transform.localRotation = Quaternion.Euler(0, 180, 0);
        cloneObj.transform.localPosition = new Vector3(0, 0, +0.001f);
        // flip original sprite
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


        // Make Editor save changes by marking the prefab or scene as dirty ¬‿¬
        if (PrefabUtility.IsPartOfPrefabInstance(gameObject))
        {
            PrefabUtility.RecordPrefabInstancePropertyModifications(gameObject);
        }
        else
        {
            EditorUtility.SetDirty(gameObject);
        }

        DestroyImmediate(this);
    }
#endif
}