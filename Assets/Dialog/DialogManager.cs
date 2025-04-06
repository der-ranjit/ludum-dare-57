using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogManager : MonoBehaviour
{
        
    [SerializeField] private TMPro.TextMeshProUGUI textComponent;
    [SerializeField] private GameObject dialogContainer; // Reference to the dialog UI GameObject

    private bool isRunning = false;

    private Queue<string> sentences;

    private GameObject currentSpeaker;
    private string currentName = "";
    private string currentText = "";

    private float currentAge = 0.0f;

    private float opacity = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        textComponent.text = "Test.";
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
        opacity = Mathf.Min(1, opacity + Time.deltaTime * 2); // Fade in
        SetUIOpacity(opacity);

        currentAge += Time.deltaTime;
        int charactersToDisplay = Mathf.FloorToInt(currentAge * 20);
        string displayText = currentText.Substring(0, Mathf.Min(charactersToDisplay, currentText.Length));
        textComponent.text = currentName + ": " + displayText;

        bool isFullText = displayText == currentText;
        if (isFullText && Input.GetKeyDown(KeyCode.Space))
        {
            DisplayNextSentence();
        }
        else if (!isFullText && Input.GetKeyDown(KeyCode.Space))
        {
            currentAge = 999999; // Set to a large value to display the full text immediately
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
            }
            else
            {
                Debug.LogError("Speaker not found: " + name);
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
