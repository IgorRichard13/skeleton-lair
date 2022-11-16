using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace R2 { 
    public class ParryCollider : MonoBehaviour
    {
        IParryable owner;

        private void Start()
        {
            owner = transform.GetComponentInParent<IParryable>();
        }
        
        private void OnTriggerEnter(Collider other)
        {
            IParryable parryable = other.transform.GetComponentInParent<IParryable>();

            if(parryable != null)
            {
                if (parryable != owner)
                {
                    parryable.OnParried(owner.GetTransform().position - parryable.GetTransform().position);
                }
            }
        }
    }
}