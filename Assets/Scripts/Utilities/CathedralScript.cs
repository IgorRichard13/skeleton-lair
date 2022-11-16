using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace R2 { 
    public class CathedralScript : MonoBehaviour, IInteractable
    {
        GeneralStatusController generalStatusController;
        public GameObject firstScene;
        public GameObject secondScene;
        public GameObject continueButton1;
        public GameObject continueButton2;
        
        private void Start()
        {
            generalStatusController = GeneralStatusController.singleton;
            gameObject.layer = 15;
        }
        
        public InteractionType GetInteractionType()
        {
            return InteractionType.cathedral;
        }

        public void OnInteract(InputManager inp)
        {
            if (Level1Handler.singleton.canPassLevel)
            {
                continueButton2.SetActive(false);
                secondScene.SetActive(false);

                firstScene.SetActive(true);
                continueButton1.SetActive(true);
                
                LevelManager.singleton.LoadScene(2);
            }
        }
    }
}