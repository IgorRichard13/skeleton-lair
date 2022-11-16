using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace R2
{
    public class WeaponHook : MonoBehaviour
    {
        public GameObject damageCollider;

        public void DamageColliderStatus(bool status){
            if(damageCollider != null)
            {
                damageCollider.SetActive(status);
            }
        }
    }
}
