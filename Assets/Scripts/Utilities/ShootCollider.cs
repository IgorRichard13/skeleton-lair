using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder;

namespace R2 { 
    public class ShootCollider : MonoBehaviour
    {
        private float counter = 10f;
        public Controller currentTarget;
        
        private void Update()
        {
            if(transform.position.y < 900)
            {
                counter -= Time.deltaTime;
                if (counter <= 0)
                {
                    Destroy(gameObject);
                }
            }
        }
        
        private void OnTriggerEnter (Collider other)
        {
            IDamageable damageable = other.GetComponentInParent<IDamageable>();
            if (damageable != null)
            {
                ActionContainer actionContainer = new()
                {
                    damage = 30,
                    owner = transform,
                    overrideReactAnim = false,
                    reactAnim = ""
                };

                damageable.OnDamage(actionContainer);
            }

            Destroy(gameObject);
        }

        public void Direction()
        {
            if (currentTarget != null)
            {
                Vector3 dir = currentTarget.mTransform.position - transform.position;
                Debug.DrawLine(currentTarget.mTransform.position, transform.position, Color.red, 5f);

                GetComponent<Rigidbody>().AddForce(dir.normalized * 0.02f);
            }
        }
            
    }
}