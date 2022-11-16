using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace R2
{
    public class FlutuantItem : MonoBehaviour
    {
        Vector3 initialPosition;
        bool animatingUp = true;

        private void Start()
        {
            initialPosition = transform.position;
            gameObject.layer = 15;
        }

        public void FixedUpdate()
        {
            if (isActiveAndEnabled)
            {
                if (transform.position.y >= initialPosition.y + 0.2f)
                {
                    animatingUp = false;
                }
                else if (transform.position.y <= initialPosition.y - 0.2f)
                {
                    animatingUp = true;
                }

                if (animatingUp)
                {
                    transform.position = new Vector3(transform.position.x, transform.position.y + 0.005f, transform.position.z);
                }
                else
                {
                    transform.position = new Vector3(transform.position.x, transform.position.y - 0.005f, transform.position.z);
                }
            }
        }
    }
}