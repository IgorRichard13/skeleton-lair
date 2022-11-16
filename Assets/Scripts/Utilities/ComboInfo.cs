using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace R2{
public class ComboInfo : StateMachineBehaviour
{
    public Combo[] combos;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Controller controller = animator.GetComponentInParent<Controller>();
        if(controller != null)
        {
            controller.LoadCombos(combos);
        }
    }
}
}