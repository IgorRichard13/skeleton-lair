using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace R2
{
    public interface ILockable
    {
        bool IsAlive();
        Transform GetLockOnTarget(Transform from);
    }

    public interface IDamageable
    {
        void OnDamage(ActionContainer action);
    }

    public interface IInteractable
    {
        void OnInteract(InputManager inp);
        InteractionType GetInteractionType();
    }

    public interface IHaveAction
    {
        ActionContainer GetActionContainer(); 
    }

    public interface IParryable
    {
        void OnParried(Vector3 dir);
        Transform GetTransform();
        void GetParried(Vector3 origin, Vector3 direction);
        bool CanBeParried();
        bool CanBeBackstabbed();
        void GetBackstabbed(Vector3 origin, Vector3 direction);
    }
}