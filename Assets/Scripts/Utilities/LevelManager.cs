using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

namespace R2 { 
    public class LevelManager : MonoBehaviour
    {
        public static LevelManager singleton;
        public GameObject LoadingScreen;
        public Slider LoadingBarFill;
        public float speed;
        public GameObject continueButton;

        private void Start()
        {
            singleton = this;
            if (SceneManager.GetActiveScene().name != "MainMenu")
            {
                EventSystem.current.SetSelectedGameObject(null);
                EventSystem.current.SetSelectedGameObject(continueButton);
            }
        }

        public void LoadScene(int sceneId)
        {
            StartCoroutine(LoadSceneAsync(sceneId));
        }

        IEnumerator LoadSceneAsync(int sceneId)
        {

            AsyncOperation operation = SceneManager.LoadSceneAsync(sceneId);

            LoadingScreen.SetActive(true);

            while (!operation.isDone)
            {
                float progressValue = Mathf.Clamp01(operation.progress / speed);
                LoadingBarFill.value = progressValue;

                yield return null;
            }
        }

        public void Continue()
        {
            LoadingScreen.SetActive(false);
        }
    }
}