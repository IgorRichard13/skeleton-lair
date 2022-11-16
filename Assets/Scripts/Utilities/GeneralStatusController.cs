using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace R2 { 
    public class GeneralStatusController : MonoBehaviour
    {
        public static GeneralStatusController singleton;
        public int killedSkeletonsCount;
        
        public bool isPaused = false;
        public bool isDead = false;
        public static bool easyMode = false;
        public bool isImortal = false;
        public bool isDebugging = false;
        
        public GameObject player;
        public Controller playerController;
        public Slider healthBar;

        public TMP_Text potionCounterText;
        public GameOverScreen gameOverScreen;
        public int potionCounter = 5;

        private void Start()
        {
            singleton = this;

            killedSkeletonsCount = 0;

            playerController = player.GetComponent<Controller>();
            potionCounterText.text = potionCounter.ToString();
        }

        private void FixedUpdate()
        {
            healthBar.value = playerController.health;
        }
        
        public void GameOver()
        {
            isDead = true;
            gameOverScreen.Setup();
        }

        public void UsePotion()
        {
            int h;
            if(easyMode)
            {
                h = 400;
            }
            else
            {
                h = 250;
            }
            
            if (playerController.health < h)
            {
                if (potionCounter > 0)
                {
                    potionCounter--;
                    if(easyMode)
                    {
                        playerController.health += 300;
                        if (playerController.health > 400)
                        {
                            playerController.health = 400;
                        }
                    }
                    else
                    {
                        playerController.health += 100;
                        if (playerController.health > 250)
                        {
                            playerController.health = 250;
                        }
                    }

                    UpdatePotionCounter();
                }
            }
        }

        public void EasyHandler()
        {
            if(easyMode)
            {
                PlayerPrefs.SetInt("EasyMode", 1);
                healthBar.maxValue = 400;
                potionCounter = 10;
                potionCounterText.text = potionCounter.ToString();
            }
        }

        public void UpdatePotionCounter()
        {
            potionCounterText.text = potionCounter.ToString();
        }
    }
}