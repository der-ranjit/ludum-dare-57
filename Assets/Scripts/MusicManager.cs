using UnityEngine;

public class MusicManager : MonoBehaviour
{
    private static MusicManager _instance;
    public static MusicManager Instance { get { return _instance; } }
    
    private AudioSource audioSource;
    
    [SerializeField] private AudioClip backgroundMusic;
    [SerializeField] private bool playOnAwake = true;
    [SerializeField] private float volume = 0.5f;
    [SerializeField] private KeyCode toggleKey = KeyCode.M;
    
    private void Awake()
    {
        // Singleton pattern
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        _instance = this;
        DontDestroyOnLoad(gameObject); // Keep across scenes
        
        // Setup audio source
        audioSource = gameObject.AddComponent<AudioSource>();
        
        // Load the music if not set
        if (backgroundMusic == null)
        {
            backgroundMusic = Resources.Load<AudioClip>("music");
        }
        
        audioSource.clip = backgroundMusic;
        audioSource.loop = true; // Set to loop
        audioSource.volume = volume;
        
        if (playOnAwake)
        {
            audioSource.Play();
        }
    }
    
    private void Update()
    {
        // Toggle music on M key press
        if (Input.GetKeyDown(toggleKey))
        {
            ToggleMusic();
        }
    }
    
    public void ToggleMusic()
    {
        if (audioSource.isPlaying)
        {
            audioSource.Pause();
        }
        else
        {
            audioSource.Play();
        }
    }
}