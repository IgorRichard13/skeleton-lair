using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace R2 { 
    public class OnStateEnterGameOver : StateMachineBehaviour
    {
        GeneralStatusController generalStatusController;
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            generalStatusController = GeneralStatusController.singleton;
            generalStatusController.GameOver();
        }
    }
}