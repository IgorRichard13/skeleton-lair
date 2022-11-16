using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace R2 { 
    public class GameUI : MonoBehaviour
    {
        public static GameUI singleton;
        public GeneralStatusController generalStatusController;
        public GameObject pauseFirstButton, optionsFirstButton, optionsCloseButton;

        public GameObject pickupText;
        public GameObject cathedralText;
        public GameObject canPassLevel;
        public GameObject talkDeathText;
        public GameObject talkText;
        public GameObject readText;

        private void Start()
        {
            singleton = this;
            //generalStatusController = GeneralStatusController.singleton;
        }

        private void Awake()
        {
            singleton = this;
        }

        public void ResetInteraction()
        {
            pickupText.SetActive(false);
            cathedralText.SetActive(false);
            canPassLevel.SetActive(false);
            talkDeathText.SetActive(false);
            talkText.SetActive(false);
            readText.SetActive(false);
        }

        public void LoadInteraction(InteractionType interactionType)
        {
            switch (interactionType)
            {
                case InteractionType.pickup:
                    pickupText.SetActive(true);
                    break;
                case InteractionType.open:
                    break;
                case InteractionType.talk:
                    talkText.SetActive(true);
                    break;
                case InteractionType.cathedral:
                    if (Level1Handler.singleton.canPassLevel)
                    {
                        canPassLevel.SetActive(true);
                    }
                    else
                    {
                        cathedralText.SetActive(true);
                    }
                    break;
                case InteractionType.statue:
                    talkDeathText.SetActive(true);
                    break;
                case InteractionType.read:
                    readText.SetActive(true);
                    break;
                default:
                    break;
            }
        }

        public void OpenOptions()
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(optionsFirstButton);
        }

        public void CloseOptions()
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(optionsCloseButton);
        }
    }
}