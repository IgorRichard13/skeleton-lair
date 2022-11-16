using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace R2 {
    public class KeyScript : MonoBehaviour, IInteractable
    {
        Level1Handler l1h;
        [SerializeField] public AudioSource keyObtained;
        public bool used = false;
        
        private void Start()
        {
            l1h = Level1Handler.singleton;
        }
        
        public InteractionType GetInteractionType()
        {
            return InteractionType.pickup;
        }

        public void OnInteract(InputManager inp)
        {
            l1h.OpenGate();
            gameObject.SetActive(false);
            used = true;
            l1h.doctor.SetActive(false);
            keyObtained.Play();
        }
    }
}