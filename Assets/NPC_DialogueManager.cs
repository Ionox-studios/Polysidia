using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class NPC_DialogueManager : MonoBehaviour {
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI dialogueText;

    private Queue<string> sentences; 
    private AudioSource audioSource; // Audio source for the audio clips
    private Queue<AudioClip> audioClips; // Queue for audio clips
    private float dialogueStartDelay = 5f;
    private float delayTimer;
    private bool dialogueStarted = false;
    public NPC_DialogueTrigger trigger;
      private float dialogueInterval = 30f; // Time between dialogues
    private float intervalTimer;
    void Start() {
        audioSource = GetComponent<AudioSource>();
        sentences = new Queue<string>();
        audioClips = new Queue<AudioClip>(); // Initialize the audio clips queue
        delayTimer = dialogueStartDelay;
    }

    void Update() {
        if (dialogueStarted) {
            intervalTimer += Time.deltaTime;
            if (intervalTimer >= dialogueInterval && sentences.Count > 0) {
                Debug.Log("Interval timer reached");
                intervalTimer = 0;
                PlayDialogue(); // Method to start a new dialogue
            }
        }
        else{
            delayTimer -= Time.deltaTime;
            if (delayTimer <= 0) {
                Debug.Log("Delay timer reached");
                trigger.TriggerDialogue();
                dialogueStarted = true;
                intervalTimer = 0;
            }
        }


        }
    

    public void PlayDialogue()
    {
        DisplayNextSentence();
    }
    public void StartDialogue (Dialogue dialogue)
    {
        Debug.Log("Starting dialogue");
        dialogueStarted = true;
        nameText.text = dialogue.name;

        sentences.Clear();
        audioClips.Clear(); // Clear the audio clips queue
    // Create a list of indices
    List<int> indices = new List<int>();
    for (int i = 0; i < dialogue.sentences.Length; i++)
    {
        indices.Add(i);
    }

    // Shuffle the indices
    Shuffle(indices);

    // Enqueue sentences and audio clips using the shuffled indices
    foreach (int index in indices)  
        {
            sentences.Enqueue(dialogue.sentences[index]);
            audioClips.Enqueue(dialogue.audioClips[index]); // Enqueue audio clips
        }
        Debug.Log(sentences.Count);
        DisplayNextSentence();
    }

 
    public void DisplayNextSentence ()
    
    {
         Debug.Log(sentences.Count);
        if (sentences.Count == 0)
        {
            EndDialogue();
            return;
        }

        string sentence = sentences.Dequeue();
        AudioClip clip = audioClips.Dequeue(); // Dequeue the audio clip
        PlayAudioClip(clip); // Play the audio clip
        StopAllCoroutines();
        StartCoroutine(TypeSentence(sentence));
    }

    void PlayAudioClip(AudioClip clip)
    {
    if (audioSource == null)
    {
        Debug.LogError("AudioSource is not assigned.");
        return;
    }

    if (clip != null)
    {
        audioSource.Stop(); // Stop any currently playing audio
        audioSource.clip = clip;
        audioSource.Play(); // Play the audio clip
    }
}
    IEnumerator TypeSentence (string sentence)
    {
        dialogueText.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            dialogueText.text += letter;
            for (int i = 0; i < 1; i++) //change typing speed here
            {
                yield return null;
            }
            
        }

    }
    
    void EndDialogue()
    {
        Debug.Log("End of conversation.");
    }
    // Fisher-Yates shuffle algorithm
    private void Shuffle(List<int> list)
    {
    int n = list.Count;
    for (int i = n - 1; i > 0; i--)
    {
        int j = Random.Range(0, i + 1);
        int temp = list[i];
        list[i] = list[j];
        list[j] = temp;
    }
}
}