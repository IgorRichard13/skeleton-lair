using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace R2 { 
    public class ScenarioScript : MonoBehaviour
    {
        void Start()
        {
            Level1Handler.singleton.InitializeEnemys();
        }
    }
}