using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreen : MonoBehaviour
{
    
    void Update()
    {
        // Check if any key was pressed
        if (Input.anyKeyDown)
        {
            // Delete own game object
            Destroy(gameObject);
        }
    }
}