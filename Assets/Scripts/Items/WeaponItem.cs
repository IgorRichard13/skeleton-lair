using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace R2
{
    [CreateAssetMenu(menuName = "R2/Items/WeaponItem")]
    public class WeaponItem : Item {
        public GameObject modelPrefab;
        public string oneHanded_anim = "Empty";
        public string twoHanded_anim = "Two Handed";
        public ItemActionContainer[] itemActions;

        [System.NonSerialized]
        public WeaponHook weaponHook;
    }
}
