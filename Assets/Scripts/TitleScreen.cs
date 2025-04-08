using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreen : MonoBehaviour
{
    [SerializeField] private string gameSceneName = "SampleScene";
    
    void Update()
    {
        // Check if any key was pressed
        if (Input.anyKeyDown)
        {
            SceneManager.LoadScene(gameSceneName);
        }
    }
}