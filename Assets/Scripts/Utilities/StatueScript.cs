using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace R2 { 
    public class StatueScript : MonoBehaviour, IInteractable
    {
        Level1Handler l1h;
        public DialogueTrigger dialogueTrigger;
        public DialogueManager dialogueManager;

        public float dialogueCounter = 0;
        
        bool firstTime = true;

        private void Start()
        {
            l1h = Level1Handler.singleton;
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
            return InteractionType.statue;
        }

        public void OnInteract(InputManager inp)
        {
            if (firstTime)
            {
                l1h.enemies[1].SetActive(true);
                firstTime = false;
            }
            
            if (dialogueCounter <= 0)
            {
                dialogueCounter = 1f;
                if (dialogueManager.isChatting == false)
                {
                    Debug.Log("trigger");
                    dialogueTrigger.TriggerDialogue();
                }
                else
                {
                    Debug.Log("nextsentence");
                    dialogueManager.DisplayNextSentence();
                }
            }
        }
    }
}