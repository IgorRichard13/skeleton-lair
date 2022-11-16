using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace R2 { 
    public class Break : MonoBehaviour
    {
        public GameObject originalObject;
        public GameObject fracturedObject;
        public AudioSource breakAudio;
        public new GameObject collider;

        private GameObject _fractObj;

        private void Start()
        {
            collider = this.gameObject;
        }

        private void OnTriggerEnter(Collider collider)
        {
            if(collider.name == "Player Controller")
            {
                Animator anim = collider.GetComponentInChildren<Animator>();
                if (anim.GetBool("isRolling"))
                {
                    BreakTheBench();
                }
            }
        }

        public void BreakTheBench()
        {
            if(originalObject != null)
            {
                originalObject.SetActive(false);

                if (fracturedObject != null)
                {
                    _fractObj = Instantiate(fracturedObject, transform.position, transform.rotation) as GameObject;

                    foreach (Transform child in _fractObj.transform)
                    {
                        if(child != null)
                        {
                           child.GetComponent<Rigidbody>().AddForce(Vector3.up * 2); 
                        }
                    }
                    
                    if (breakAudio != null)
                    {
                        breakAudio.Play();
                    }

                    Destroy(gameObject);
                }
            }
        }
    }
}
