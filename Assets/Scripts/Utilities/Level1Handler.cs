using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace R2
{
    public class Level1Handler : MonoBehaviour
    {
        public static Level1Handler singleton;
        GeneralStatusController gsc;
        
        public GameObject key;
        public GameObject doctor;
        KeyScript keyScript;
        public bool canPassLevel = false;
        public bool keyCamEvent = false;
        public GameObject enemySample;
        public GameObject[] enemies;
        public GameObject[] gates;
        public TMP_Text counter;

        private void Start()
        {
            singleton = this;
            gsc = GeneralStatusController.singleton;
            keyScript = key.GetComponent<KeyScript>();
        }

        private void FixedUpdate()
        {
            if (keyScript.used == false)
            {
                if (!keyCamEvent)
                {
                    //camera event to show the key
                    
                    key.SetActive(gsc.killedSkeletonsCount >= 15);
                    doctor.SetActive(gsc.killedSkeletonsCount >= 15);
                }
            }

            if (gsc.killedSkeletonsCount > 60)
            {
                canPassLevel = true;
            }

            counter.text = gsc.killedSkeletonsCount.ToString();
        }

        public void InitializeEnemys()
        {
            enemies[0].SetActive(true);
            enemies[1].SetActive(false);
            enemies[2].SetActive(false);
        }
        
        public void OpenGate()
        {
            gates = GameObject.FindGameObjectsWithTag("Gate");
            for (int i = 0; i < gates.Length; i++)
            {
                Animator gateAnim = gates[i].GetComponent<Animator>();

                if (gates[i].name == "Gate_2")
                {
                    gateAnim.Play("Gate_2_Open");
                }

                if (gates[i].name == "Gate_3")
                {
                    gateAnim.Play("Gate_3_Open");
                }

                if (gates[i].name == "Plane")
                {
                    gates[i].SetActive(false);
                }
            }

            enemies[2].SetActive(true);
        }
    }
}
