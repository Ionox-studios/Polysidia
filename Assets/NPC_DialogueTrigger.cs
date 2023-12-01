using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NPC_DialogueTrigger : MonoBehaviour
{
   public Dialogue dialogue;

   public void TriggerDialogue ()
   {
      FindObjectOfType<NPC_DialogueManager>().StartDialogue(dialogue);
   }
}