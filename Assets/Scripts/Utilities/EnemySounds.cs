using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace R2 { 
    public class EnemySounds : MonoBehaviour
    {
        public static EnemySounds singleton;

        private void Start()
        {
            singleton = this;
        }

        [SerializeField] public AudioSource deathSound;
        [SerializeField] public AudioSource hitSound;
        [SerializeField] public AudioSource attackSound;
        [SerializeField] public AudioSource BossDeathSound;
        [SerializeField] public AudioSource BossHitSound;
        [SerializeField] public AudioSource BossAttackSound;
    }   
}