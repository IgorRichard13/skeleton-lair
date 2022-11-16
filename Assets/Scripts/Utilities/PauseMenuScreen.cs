using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace R2 { 
    public class PauseMenuScreen : MonoBehaviour
    {
        public static PauseMenuScreen singleton;
        GeneralStatusController generalStatusController;

        private void Start()
        {
            singleton = this;
            generalStatusController = GeneralStatusController.singleton;
        }
        
        public void ResumeButton()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            generalStatusController.isPaused = false;
            gameObject.SetActive(false);
        }
        
        public void MenuButton()
        {
            SceneManager.LoadScene("MainMenu");
        }
    }
}