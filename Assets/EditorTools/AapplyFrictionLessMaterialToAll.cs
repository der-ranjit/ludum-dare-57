#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public class ApplyFrictionlessMaterial : EditorWindow
{
    private PhysicMaterial frictionlessMaterial;

    [MenuItem("Tools/Apply Frictionless Material to Prefabs")]
    public static void ShowWindow()
    {
        GetWindow<ApplyFrictionlessMaterial>("Apply Frictionless Material");
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("Apply Frictionless Material to Prefabs", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        // Select the frictionless material
        frictionlessMaterial = (PhysicMaterial)EditorGUILayout.ObjectField("Frictionless Material", frictionlessMaterial, typeof(PhysicMaterial), false);

        EditorGUILayout.Space();

        if (GUILayout.Button("Apply to All Prefabs"))
        {
            if (frictionlessMaterial == null)
            {
                Debug.LogError("Please assign a frictionless material before applying.");
                return;
            }

            ApplyMaterialToPrefabs();
        }
    }

    private void ApplyMaterialToPrefabs()
    {
        // Find all prefab assets in the project
        string[] prefabGuids = AssetDatabase.FindAssets("t:Prefab");
        int updatedCount = 0;

        foreach (string guid in prefabGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);

            if (prefab == null) continue;

            // Check all colliders in the prefab
            bool prefabModified = false;
            Collider[] colliders = prefab.GetComponentsInChildren<Collider>(true);
            foreach (Collider collider in colliders)
            {
                if (collider.material == null)
                {
                    collider.material = frictionlessMaterial;
                    prefabModified = true;
                }
            }

            // Save changes to the prefab if modified
            if (prefabModified)
            {
                updatedCount++;
                PrefabUtility.SavePrefabAsset(prefab);
                Debug.Log($"Updated prefab: {path}");
            }
        }

        Debug.Log($"Finished applying frictionless material. Updated {updatedCount} prefabs.");
    }
}
#endif