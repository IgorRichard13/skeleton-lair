using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace R2
{
    [System.Serializable]
    public class ItemActionContainer
    {
        public string animName;
        public ItemAction itemAction;
        public AttackInputs attackInput;
        public bool isMirrored;
        public bool isTwoHanded;
        public Item itemActual;
        public WeaponHook weaponHook;
        public int damage = 20;
        public bool overrideReactAnim;
        public string reactAnim;
        public bool canParry;
        public bool canBackstab;

        public void ExecuteItemAction(Controller controller)
        {
            itemAction.ExecuteAction(this, controller);
        }
    }
}
