using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

namespace R2 {
    public class MenuScript : MonoBehaviour
    {
        public GameObject pauseFirstButton, optionsFirstButton, optionsCloseButton;

        public void ExitButton()
        {
            Application.Quit();
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