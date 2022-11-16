using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace R2
{
    public class DamageCollider : MonoBehaviour
    {
        IHaveAction owner;
        private void Start()
        {
            owner = GetComponentInParent<IHaveAction>();
            ActionContainer actionContainer = owner.GetActionContainer();
            if (actionContainer == null)
            {
                Debug.Log("DamageCollider: No action container found on owner");
                return;
            }
        }
        
        private void OnTriggerEnter(Collider other)
        {
            owner ??= GetComponentInParent<IHaveAction>();
            
            IDamageable damageable = other.GetComponentInParent<IDamageable>();
            if (damageable != null )
            {
                damageable.OnDamage(owner.GetActionContainer());
            }
        }
    }
}