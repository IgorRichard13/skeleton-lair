using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace R2 { 
    public class DebugMenu : MonoBehaviour
    {
        public static DebugMenu singleton;
        GeneralStatusController generalStatusController;
        public GameObject backButton;
        public Slider PlayerHealthSlider;
        public Slider KilledSkeletonsSlider;
        public Slider PotionCounterSlider;
        public Toggle ImortalToggle;
        public TMP_Text PotionCounterText;
        public TMP_Text PlayerHealthText;
        public TMP_Text KilledSkeletonsText;
        
        private void Start()
        {
            singleton = this;
            generalStatusController = GeneralStatusController.singleton;
        }

        public void Update()
        {
            PotionCounterText.SetText("Health Potions: " + generalStatusController.potionCounter.ToString());
            PlayerHealthText.SetText("Player Health: " + generalStatusController.playerController.health.ToString());
            KilledSkeletonsText.SetText("Killed Skeletons: " + generalStatusController.killedSkeletonsCount.ToString());
        }

        public void Initialize()
        {
            generalStatusController ??= GeneralStatusController.singleton;
            generalStatusController.isDebugging = true;
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(backButton);
            PlayerHealthSlider.value = generalStatusController.playerController.health;
            KilledSkeletonsSlider.value = generalStatusController.killedSkeletonsCount;
            PotionCounterSlider.value = generalStatusController.potionCounter;
            ImortalToggle.isOn = generalStatusController.isImortal;

            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
        }

        public void SetHealth(float health)
        {
            generalStatusController.playerController.health = Mathf.FloorToInt(health);
        }

        public void SetKilledSkeletons(float count)
        {
            generalStatusController.killedSkeletonsCount = Mathf.FloorToInt(count);
        }

        public void SetHealthPotions(float count)
        {
            generalStatusController.potionCounter = Mathf.FloorToInt(count);
            generalStatusController.UpdatePotionCounter();
        }

        public void SetImortal(bool status)
        {
            generalStatusController.isImortal = status;
        }

        public void KillAllEnemies()
        {
            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
            foreach (GameObject enemy in enemies)
            {
                enemy.GetComponent<AIController>().Die();
            }
        }

        public void TeleportToBonfire()
        {
            // Coordinates:
            // X : 445.5278 - 363.5146 = 82.0132
            // Y : 430.8992 - 372.4692 = 58.43
            // Z : 37.97159 + 40.15805 = 78.12964

            generalStatusController.player.transform.position = new Vector3(82.0132f, 58.43f, 78.12964f);
        }

        public void TimeScale1()
        {
            Time.timeScale = 1;
        }

        public void TimeScale3()
        {
            Time.timeScale = 3;
        }

        public void StopDebugging()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            generalStatusController.isDebugging = false;
            gameObject.SetActive(false);
        }
    }
}