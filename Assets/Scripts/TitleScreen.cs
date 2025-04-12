using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreen : MonoBehaviour
{
    
    void Update()
    {
        // Check if any key was pressed
        if (Input.anyKeyDown && 
            !Input.GetMouseButtonDown(0))
        {
            // Start fade
            GameManager.Instance.ProceedToNextRoomIn(0.0001f);
            // Delete own game object
            Destroy(gameObject);
        }
    }
}