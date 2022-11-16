using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace R2 { 
    public class DialogueManager : MonoBehaviour
    {
        public static DialogueManager singleton;
        public Controller controller;
        public TMP_Text nameText;
        public TMP_Text dialogueText;
        public Transform dialoguingCharacter;

        public Animator animator;

        private Queue<string> sentences;

        public bool isChatting;
        
        void Start()
        {
            singleton = this;
            sentences = new Queue<string>();
            controller = GameObject.Find("Player Controller").GetComponent<Controller>();
        }

        private void Update()
        {
            if(isChatting)
            {
                float distance = Vector3.Distance(dialoguingCharacter.position, controller.transform.position);
                if (distance > 2.5f)
                {
                    EndDialogue();
                }
            }
        }

        public void StartDialogue(Dialogue dialogue, Transform transform)
        {
            isChatting = true;
            dialoguingCharacter = transform;
            animator.SetBool("isOpen", true);
            nameText.text = dialogue.name;
            
            sentences.Clear();

            foreach (string sentence in dialogue.sentences)
            {
                sentences.Enqueue(sentence);
            }

            DisplayNextSentence();
        }

        public void DisplayNextSentence()
        {
            if (sentences.Count == 0)
            {
                EndDialogue();
                return;
            }

            string sentence = sentences.Dequeue();
            StopAllCoroutines();
            StartCoroutine(TypeSentence(sentence));
        }

        IEnumerator TypeSentence (string sentence)
        {
            dialogueText.text = "";
            foreach (char letter in sentence.ToCharArray())
            {
                dialogueText.text += letter;
                yield return null;
            }
        }

        void EndDialogue()
        {
            
            isChatting = false;
            animator.SetBool("isOpen", false);
        }
    }
}