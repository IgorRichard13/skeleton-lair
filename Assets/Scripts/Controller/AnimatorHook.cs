using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace R2
{
    public class AnimatorHook : MonoBehaviour
    {
        Controller controller;
        Animator animator;
        bool isAI;

        public Vector3 deltaPosition;
        public Vector3 lookAtPosition;
        public bool hasLookAtTarget;
        public bool canBeParried;
        public bool canRotate;
        public bool canDoCombo;
        public bool openDamageCollider = false;
        public bool canMove;
        public bool disableIK;

        public void Start(){
            animator = GetComponentInChildren<Animator>();
            controller = GetComponentInParent<Controller>();
            if(controller == null){
                isAI = true;
            }else{
                isAI = false;
            }

            RagdollStatus(false);
        }

        void RagdollStatus(bool status)
        {
            Rigidbody[] ragdollRigids = GetComponentsInChildren<Rigidbody>();
            Collider[] ragdollColliders = GetComponentsInChildren<Collider>();

            foreach (Rigidbody rb in ragdollRigids)
            {
                rb.isKinematic = !status;
                rb.gameObject.layer = 10;
            }

            foreach (Collider col in ragdollColliders)
            {
                col.isTrigger = !status;
            }

            animator.enabled = !status;
        }

        public void OnAnimatorMove() {
            OnAnimatorMoveOverride();
        }

        protected virtual void OnAnimatorMoveOverride(){
            float delta = Time.deltaTime;

            if(!isAI){
                if(controller == null) return;
                if(controller.isInteracting == false){
                    return;
                }
                if(controller.isGrounded && delta > 0){
                    deltaPosition = (animator.deltaPosition) / delta;
                }
            }
            else{
                deltaPosition = (animator.deltaPosition) / delta;
            }
        }

        public void OnAnimatorIK(int layerIndex){
            if (hasLookAtTarget && !disableIK)
            {
                animator.SetLookAtWeight(1, .9f, .95f, 1, 1);
                animator.SetLookAtPosition(lookAtPosition);
            }
        }

        public void OpenCanMove(){
            canMove = true;
        }
    
        public void OpenDamageColliders(){
            openDamageCollider = true;
        }

        public void CloseDamageColliders(){
            openDamageCollider = false;
        }   

        public void EnableCombo(){
            canDoCombo = true;
        }

        public void DisableCombo()
        {
            canDoCombo = false;
        }

        public void EnableRotation(){
            canRotate = true;
        }

        public void DisableRotation(){
            canRotate = false;
        }

        public void EnableRagdoll()
        {
            RagdollStatus(true);
        }

        public void EnableParryCollider()
        {
            if(!isAI)
            {
                controller.OpenParryCollider();
            }
        }

        public void EnableCanBeParried()
        {
            canBeParried = true;
        }

        public void DisableCanBeParried()
        {
            canBeParried = false;
        }
        
        public void DisableIK()
        {
            disableIK = true;
        }

        public void EnableIK()
        {
            disableIK = false;
        }

        public void Shoot()
        {
            if(isAI)
            {
                AIController ai = GetComponentInParent<AIController>();
                ai.BossShoot(1);
            }
        }

        public void Shoot2()
        {
            if (isAI)
            {
                AIController ai = GetComponentInParent<AIController>();
                ai.BossShoot(1);
            }
        }

        public void SpawnEnemies()
        {
            {
                if (isAI)
                {
                    AIController ai = GetComponentInParent<AIController>();
                    ai.SpawnEnemies();
                }
            }
        }

        public void PlayAttackSound()
        {
            if(isAI)
            {
                AIController ai = GetComponentInParent<AIController>();
                ai.PlayAttackSound();
            }
            else
            {
                controller.PlayAttackSound();
            }
        }
    }
}
