using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace R2 {
    public class PickableHook : MonoBehaviour, IInteractable
    {
        public Item targetItem;
        public InteractionType intType;
        public AudioSource audio;

        void Start()
        {
            gameObject.layer = 15;
        }
        
        public void OnInteract(InputManager inp)
        {
            audio.Play();
            if(targetItem is WeaponItem)
            {
                inp.controller.LoadWeapon(targetItem, false);
            }
            else if (targetItem is ClothItem)
            {
                inp.controller.LoadCloth(targetItem);
            }
            gameObject.SetActive(false);
        }

        public InteractionType GetInteractionType()
        {
            return intType;
        }
    }
}