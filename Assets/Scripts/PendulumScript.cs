using UnityEngine;

public class PendulumScript : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Destroy(this);
            // TODO: unlock the door
        }
    }
}