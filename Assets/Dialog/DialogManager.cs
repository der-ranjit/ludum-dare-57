using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogManager : MonoBehaviour
{
    public static DialogManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
        
    [SerializeField] private TMPro.TextMeshProUGUI textComponent;
    [SerializeField] private GameObject dialogContainer; // Reference to the dialog UI GameObject

    private bool isRunning = false;

    private Queue<string> sentences;

    private GameObject currentSpeaker;
    private string currentName = "";
    private string currentText = "";

    private float currentAge = 0.0f;
    private float waitingUntilAge = 0.0f;

    private float opacity = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        textComponent.text = "";
        // this.StartDialog(new string[] { "1: Hello, how are you?", "1: I'm fine, thank you!", "1: What about you?" });
        // Hide the dialog UI here by setting this canvas's opacity to 0 (the game object)
        SetUIOpacity(0);
    }

    // Update is called once per frame
    void Update()
    {
        if (!isRunning)
        {
            opacity = Mathf.Max(0, opacity - Time.deltaTime * 2); // Fade out
            SetUIOpacity(opacity);
            return;
        }
        if (currentText.Length > 0) {
            opacity = Mathf.Min(1, opacity + Time.deltaTime * 2); // Fade in
        }
        SetUIOpacity(opacity);

        currentAge += Time.deltaTime;

        
        if (waitingUntilAge > 0) {
            if (currentAge >= waitingUntilAge) {
                waitingUntilAge = 0;
                Debug.Log("Waiting time ended.");
                DisplayNextSentence(); // Display the next sentence after waiting
            }
            return; // Skip the rest of the update if we're waiting
        }


        int charactersToDisplay = Mathf.FloorToInt(currentAge * 35 );
        string displayText = currentText.Substring(0, Mathf.Min(charactersToDisplay, currentText.Length));
        textComponent.text = currentName + ": " + displayText;

        bool userTriesToProceed = Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space);
        bool isFullText = displayText == currentText;
        if (userTriesToProceed) {
            if (isFullText)
            {
                // If the text is fully displayed, we can proceed to the next sentence
                DisplayNextSentence();
            }
            else
            {
                // If the text is not fully displayed, we can display the full text immediately
                currentAge = 999999;
        }
        }
    }

    void SetUIOpacity(float opacity)
    {
        // We just call it opacity, but actually we move the canvas down by 300px * opacity
        float f = 0.5f - 0.5f * Mathf.Cos(Mathf.PI * opacity);
        dialogContainer.transform.localPosition = new Vector3(0, -300f * (1f - f), 0);
    }

    public void StartDialog(string[] sentencesArray)
    {
        sentences = new Queue<string>();
        isRunning = true;

        Debug.Log("Starting dialog with sentences: " + string.Join(", ", sentencesArray));

        foreach (string sentence in sentencesArray)
        {
            sentences.Enqueue(sentence);
        }

        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {
        if (sentences.Count == 0)
        {
            EndDialog();
            return;
        }

        currentAge = 0;
        string sentence = sentences.Dequeue();

        Debug.Log("Displaying sentence: " + sentence);

        // If sentence starts with "!wait", we wait for the specified time
        if (sentence.StartsWith("!wait"))
        {
            currentText = ""; // Clear the current text while waiting
            currentSpeaker = null; // Clear the current speaker while waiting
            string[] partss = sentence.Split(new[] { " " }, 2, System.StringSplitOptions.None);
            if (partss.Length == 2 && float.TryParse(partss[1], out float waitTime))
            {
                Debug.Log("Waiting for " + waitTime + " seconds.");
                waitingUntilAge = waitTime;
            }
            return;
        }
        CameraController camController = Camera.main.GetComponent<CameraController>();
        if (sentence.StartsWith("!cam"))
        {
            // If sentence starts with "!cam", we set the camera to the specified position
            if (sentence == "!cam")
            {
                // Reset cam
                camController.SetOverride();
            }
            else
            {
                string[] partss = sentence.Split(new[] { " " }, 4, System.StringSplitOptions.None);
                if (partss.Length == 4 && 
                    float.TryParse(partss[1], System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out float x) && 
                    float.TryParse(partss[2], System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out float y) && 
                    float.TryParse(partss[3], System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out float z)
                )
                {
                    Debug.Log("Setting camera override to " + x + ", " + y + ", " + z);
                    camController.SetOverride(new Vector3(x, y, z));
                    // waitingUntilAge = 1f; // Wait for 1 second before displaying the next sentence
                }
            }
            DisplayNextSentence();
            return;
        }
        if (sentence.StartsWith("!turn")) {
            // E.g. `!turn Player 42`
            string[] partss = sentence.Split(new[] { " " }, 3, System.StringSplitOptions.None);
            if (partss.Length == 3 && float.TryParse(partss[2], System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out float angle))
            {
                string name = partss[1];
                GameObject speaker = GameObject.Find(name);
                if (speaker != null)
                {
                    Debug.Log("Turning " + name + " to " + angle + " degrees.");
                    PlayerController controller = speaker.GetComponent<PlayerController>();
                    if (controller != null)
                    {
                        Debug.Log("Smooth target rotation adjustment");
                        controller.SetTargetRotation(angle);
                    } else {
                        speaker.transform.rotation = Quaternion.Euler(0, angle, 0);
                    }
                }
                else
                {
                    Debug.LogError("Speaker not found: " + name);
                }
            }
            DisplayNextSentence();
            return;
        }

        if (sentence.StartsWith("!type")) {
            bool isStarting = !sentence.Contains("stop");
            DeskScript[] desks = FindObjectsOfType<DeskScript>();
            if (isStarting) {
                foreach (DeskScript desk in desks) {
                    desk.EnableAnimation();
                }
            } else {
                foreach (DeskScript desk in desks) {
                    desk.DisableAnimation();
                }
            }
            DisplayNextSentence();
            return;
        }

        // Sentences are of form `<num>: <text>`
        string[] parts = sentence.Split(new[] { ": " }, 2, System.StringSplitOptions.None);
        if (parts.Length == 2)
        {
            int num = int.Parse(parts[0]);
            string text = parts[1];
            string name = GetNameFromNum(num);
            GameObject speaker = GameObject.Find(name);
            if (speaker != null)
            {
                currentSpeaker = speaker;
                currentName = name;
                currentText = text;
                camController.SetOverrideTarget(speaker.transform.position + new Vector3(0, -1f, 0));
            }
            else
            {
                Debug.LogError("Speaker not found: " + name);
                camController.SetOverrideTarget();
            }
            // Display the text here, e.g., in a UI Text component
            Debug.Log($"Displaying sentence {num}: {text}");
        }
        else
        {
            Debug.LogError("Invalid sentence format: " + sentence);
        }
    }

    public void EndDialog()
    {
        isRunning = false;
        sentences.Clear();
        Camera.main.GetComponent<CameraController>().SetOverride(); // Reset camera override
        // Hide the dialog UI here
        Debug.Log("Dialog ended.");
    }

    public bool IsRunning()
    {
        return isRunning;
    }

    private string GetNameFromNum(int num)
    {
        switch (num)
        {
            case 1: return "Player";
            case 2: return "Kevin";
            default: return "UnknownSpeaker";
        }   
    }
}
