using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace R2
{
    [System.Serializable]
    public class Level2Handler : MonoBehaviour
    {
        public static Level2Handler singleton;
        
        GeneralStatusController gsc;
        public GameObject boss;
        AIController bossController;
        public Slider BossHealthBar;

        private void Start()
        {
            singleton = this;
            gsc = GeneralStatusController.singleton;
            bossController = boss.GetComponent<AIController>();
        }

        private void FixedUpdate()
        {
            BossHealthBar.value = bossController.health;
        }
    }
}