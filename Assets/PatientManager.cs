using UnityEngine;
using TMPro; // Add this if you're using TextMeshPro for dialogues

public class PatientManager : MonoBehaviour
{
    public RandomizeAppearance randomizeAppearance; // Reference to the RandomizeAppearance script
    public DialogueRandomizer dialogueRandomizer; // Reference to the DialogueRandomizer script
    public TMP_Text dialogueText; // Reference to the TextMeshPro text component
    public DialoguePatInfo randomDialogue;
    void OnEnable()
    {
        // Randomize the appearance of the patient
        //randomizeAppearance.Randomize();

        // Get a random dialogue and set it to the text component
        //DialoguePatInfo randomDialogue = dialogueRandomizer.GetRandomDialogue();
        //dialogueText.text = randomDialogue.DialogueText;
    }
    public void SetRandomizer(DialogueRandomizer randomizer)
    {
        dialogueRandomizer = randomizer;
    }
        public void InitializePatient()
    {
       randomizeAppearance.Randomize();

        randomDialogue = dialogueRandomizer.GetRandomDialogue();
        dialogueText.text = randomDialogue.DialogueText;
    }
    public DialoguePatInfo GetDialogue()
    {
        return randomDialogue;
    }
}
