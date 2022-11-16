using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

namespace R2 { 
    public class GameOverScreen : MonoBehaviour
    {
        public GameObject Restart;
        public GameObject Menu;
        public void Setup()
        {
            gameObject.SetActive(true);
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(Restart);
        }

        private void Update()
        {
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
        }

        public void RestartButton()
        {
            LevelManager.singleton.LoadScene(1);
        }

        public void MenuButton()
        {
            LevelManager.singleton.LoadScene(0);
        }
    }
}