using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace R2 { 
    public class History1Scene : MonoBehaviour
    {
        public TMP_Text historyText;
        public Dialogue dialogue;
        public GameObject continue2Button;

        private void Start()
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(continue2Button);
            StartCoroutine(ShowHistory(dialogue.sentences[0]));
        }

        private IEnumerator ShowHistory(string sentence)
        {
            historyText.text = "";
            foreach (char letter in sentence.ToCharArray())
            {
                historyText.text += letter;
                WaitForFixedUpdate wait = new();
                yield return wait;
            }
        }
    }
}