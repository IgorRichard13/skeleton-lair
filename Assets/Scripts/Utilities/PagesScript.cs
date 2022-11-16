using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace R2 { 
    public class PagesScript : MonoBehaviour, IInteractable
    {
        public DialogueTrigger dialogueTrigger;
        public DialogueManager dialogueManager;

        public float dialogueCounter = 0;

        private void Start()
        {
            gameObject.layer = 15;
        }

        private void Update()
        {
            if (dialogueCounter > 0)
            {
                dialogueCounter -= Time.deltaTime;
            }
        }

        public InteractionType GetInteractionType()
        {
            return InteractionType.read;
        }

        public void OnInteract(InputManager inp)
        {
            //change to another type of dialog, maybe a page of a book, or a book itself?
            if (dialogueCounter <= 0)
            {
                dialogueCounter = 1f;
                if (dialogueManager.isChatting == false)
                {
                    dialogueTrigger.TriggerDialogue();
                }
                else
                {
                    dialogueManager.DisplayNextSentence();
                }
            }
        }
    }
}