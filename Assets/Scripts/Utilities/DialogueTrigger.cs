using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace R2 { 
    public class DialogueTrigger : MonoBehaviour
    {
        public Dialogue dialogue;

        public void TriggerDialogue()
        {
            FindObjectOfType<DialogueManager>().StartDialogue(dialogue, this.transform);
        }
    }
}