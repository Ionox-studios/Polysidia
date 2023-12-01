using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour {
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI dialogueText;
    private AudioSource audioSource; // Audio source for the audio clips
    private Queue<string> sentences;
    private Queue<AudioClip> audioClips; // Queue for audio clips
    private float dialogueStartDelay = 5f;
    private float delayTimer;
    private bool dialogueStarted = false;
    public DialogueTrigger trigger;

    void Start() {
        audioSource = GetComponent<AudioSource>();
        sentences = new Queue<string>();
        audioClips = new Queue<AudioClip>(); // Initialize the audio clips queue
        delayTimer = dialogueStartDelay;
    }

    //void Update() {
     //   if (!dialogueStarted) {
     //       delayTimer -= Time.deltaTime;
     //       if (delayTimer <= 0) {
    //          trigger.TriggerDialogue();
    //            dialogueStarted = true;
    //        }
    //    }

     //   if (audioSource.isPlaying == false && sentences.Count > 0) {
    //        DisplayNextSentence();
    //    }
    //}

    public void StartDialogue (Dialogue dialogue)
    {
        dialogueStarted = true;
        nameText.text = dialogue.name;

        sentences.Clear();
        audioClips.Clear(); // Clear the audio clips queue

        for (int i = 0; i < dialogue.sentences.Length; i++) {
            sentences.Enqueue(dialogue.sentences[i]);
            audioClips.Enqueue(dialogue.audioClips[i]); // Enqueue audio clips
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
}
