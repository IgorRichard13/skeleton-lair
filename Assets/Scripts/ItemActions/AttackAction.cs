using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace R2
{
    [CreateAssetMenu(menuName = "R2/ItemActions/AttackAction")]
    public class AttackAction : ItemAction
    {
        public override void ExecuteAction(ItemActionContainer ic, Controller cs)
        {
            // cs.AssignCurrentWeaponAndAction((WeaponItem)ic.itemActual, ic);
            cs.PlayTargetAnimation(ic.animName, true, ic.isMirrored);
        }
    }
}
