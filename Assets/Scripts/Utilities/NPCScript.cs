using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace R2 { 
    public class NPCScript : MonoBehaviour, IInteractable
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
            return InteractionType.talk;
        }

        public void OnInteract(InputManager inp)
        {
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