using UnityEditor;
using UnityEngine;

public class RoomCreatorEditor : EditorWindow
{
    private float planeWidth = 10f; // Width of the plane
    private float planeHeight = 10f; // Height of the plane
    private float wallHeight = 2f; // Height of the walls
    private Material planeMaterial; // Material for the plane
    private Material wallMaterial; // Material for the walls

    [MenuItem("Tools/Room Creator")]
    public static void ShowWindow()
    {
        GetWindow<RoomCreatorEditor>("Room Creator");
    }

    private void OnGUI()
    {
        GUILayout.Label("Room Settings", EditorStyles.boldLabel);

        // Input fields for room dimensions
        planeWidth = EditorGUILayout.FloatField("Plane Width", planeWidth);
        planeHeight = EditorGUILayout.FloatField("Plane Height", planeHeight);
        wallHeight = EditorGUILayout.FloatField("Wall Height", wallHeight);

        // Material fields
        planeMaterial = (Material)EditorGUILayout.ObjectField("Plane Material", planeMaterial, typeof(Material), false);
        wallMaterial = (Material)EditorGUILayout.ObjectField("Wall Material", wallMaterial, typeof(Material), false);

        // Generate Room button
        if (GUILayout.Button("Generate Room"))
        {
            GameObject room = RoomCreator.DeleteAndGenerateRoom(wallMaterial, planeWidth, planeHeight, planeMaterial);
            // Select the created room in the hierarchy
            Selection.activeGameObject = room;
        }

        if (GUILayout.Button("reset"))
        {
            RoomCreator.ResetRoomsCreated();
        }
    }
}