using UnityEngine;

[RequireComponent(typeof(MeshCollider))]
public class DeathTriggerPlane : MonoBehaviour
{

    public static GameObject CreateDeathPlane(GameObject plane)
    {
        // create death plane 
        GameObject deathPlane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        deathPlane.transform.localScale = plane.transform.localScale;
        deathPlane.transform.position = plane.transform.position;
        deathPlane.transform.rotation = plane.transform.rotation;
        deathPlane.AddComponent<DeathTriggerPlane>();

        return deathPlane;
    }

    private MeshCollider meshCollider;
    private void Start()
    {
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer != null)
        {
            meshRenderer.enabled = false;
        }
    
        meshCollider = GetComponent<MeshCollider>();
        meshCollider.isTrigger = true;
        meshCollider.convex = true;
        // scale the plane's (0) height down to make the collider smaller 
        Vector3 localScale = transform.localScale;
        localScale.y = 0.2f;
        transform.localScale = localScale;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<IDamageable>(out IDamageable damageable))
        {
            damageable.Die();
        }
    }
}