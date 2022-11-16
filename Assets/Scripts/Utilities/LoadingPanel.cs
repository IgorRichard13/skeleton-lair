using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace R2 { 
    public class LoadingPanel : MonoBehaviour
    {
        public GameObject continue1Button;
        private void Start()
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(continue1Button);
        }

        public void BackToMenu()
        {
            LevelManager.singleton.LoadScene(0);
        }

        public void setFastMode(bool status)
        {
            GeneralStatusController.easyMode = status;
        }
    }
}