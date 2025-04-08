using UnityEngine;

[RequireComponent(typeof(MeshCollider))]
public class DeathTriggerPlane : MonoBehaviour
{
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