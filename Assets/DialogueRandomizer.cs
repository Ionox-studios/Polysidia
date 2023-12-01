using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class DialoguePatInfo
{
    public string DialogueText;
    public float Weight;
    public string Trait;
}

[System.Serializable]
public class DialogueWrapper
{
    public List<DialoguePatInfo> dialogues;
}

public class DialogueRandomizer : MonoBehaviour
{
    public TextAsset dialoguesJson;
    private List<DialoguePatInfo> dialogues;
    private System.Random random;

    void Awake()
    {
        LoadDialogues();
        random = new System.Random();
    }

    private void LoadDialogues()
    {
        if (dialoguesJson != null)
        {
            DialogueWrapper wrapper = JsonUtility.FromJson<DialogueWrapper>(dialoguesJson.text);
            if (wrapper != null && wrapper.dialogues != null)
            {
                dialogues = wrapper.dialogues;
                Debug.Log("Dialogues loaded successfully.");
            }
            else
            {
                Debug.LogError("Failed to load or parse dialogues.");
            }
        }
        else
        {
            Debug.LogError("Dialogues JSON not assigned in the editor.");
        }
    }

    public DialoguePatInfo GetRandomDialogue()
    {
        if (dialogues != null && dialogues.Count > 0)
        {
            int index = random.Next(dialogues.Count);
            return dialogues[index];
        }
        else
        {
            Debug.LogError("Dialogues list is empty or null.");
            return null;
        }
    }
}
