using UnityEngine;

public class DirectionalLightCirculator : MonoBehaviour
{
    [Header("Rotation Settings")]
    public float rotationSpeed = 10f; // Speed of the light's rotation

    private void Update()
    {
        // Rotate the light around its Y-axis
        // transform.rotation.Set(0f, rotationSpeed * Time.deltaTime, 0f);
    }
}