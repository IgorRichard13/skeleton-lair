using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

namespace R2
{
    public class Controller : MonoBehaviour, IHaveAction, IDamageable, IParryable
    {
        public bool lockOn = false;
        public bool isOnAir = false;
        public bool isGrounded = false;
        public bool isRolling = false;
        public bool isInteracting = false;
        public bool isSprinting = false;
        public bool isDancing = false;
        public bool manualDancing = false;
        public bool isDead = false;

        [Header("Controller")]
        public float movementSpeed = 3;
        public float sprintSpeed = 5;
        public float rollSpeed = 5;
        public float adaptSpeed = 1;
        public float rotationSpeed = 10;
        public float attackRotationSpeed = 3;
        public float groundDownDistanceOnAir = .4f;
        public float frontRayOffset = .5f;
        public float groundedSpeed = 0.1f;
        public float goundedDistanceRay = .5f;
        private float velocityMultiplier = 1;

        [Header("Status")]
        public int health = 150;

        public Animator anim;
        new Rigidbody rigidbody;

        [HideInInspector]
        public Transform mTransform;
        [HideInInspector]
        public Transform currentLockTarget;
        GeneralStatusController generalStatusController;

        [SerializeField] private AudioSource deathSound;
        [SerializeField] private AudioSource hitSound;
        [SerializeField] public AudioSource attackSound;

        LayerMask ignoreForGroundCheck;

        public List<ClothItem> startingCloth;
        public ItemActionContainer[] currentActions;
        public ItemActionContainer[] defaultActions;
        ItemActionContainer currentAction;
        WeaponHolderManager weaponHolderManager;
        ClothManager clothManager;
        [HideInInspector]
        public AnimatorHook animatorHook;
        Vector3 currentNormal;

        public GameObject parryCollider;

        ActionContainer _lastAction;
        public ActionContainer LastAction
        {
            get
            {
                _lastAction ??= new ActionContainer();

                if (currentAction == null)
                {
                    _lastAction.owner = mTransform;
                    _lastAction.damage = 0;
                    _lastAction.overrideReactAnim = false;
                    _lastAction.reactAnim = "";
                }
                else
                {
                    _lastAction.owner = mTransform;
                    
                    if (GeneralStatusController.easyMode)
                    {
                        _lastAction.damage = currentAction.damage * 2;
                    }
                    else
                    {
                        _lastAction.damage = currentAction.damage;
                    }
                    
                    _lastAction.overrideReactAnim = currentAction.overrideReactAnim;
                    _lastAction.reactAnim = currentAction.reactAnim;
                }
                return _lastAction;
            }
        }

        public void SetWeapons(Item rh, Item lh)
        {
            weaponHolderManager.Init();
            LoadWeapon(rh, false);
            LoadWeapon(lh, true);
        }

        public void Init()
        {
            mTransform = this.transform;
            rigidbody = GetComponentInChildren<Rigidbody>();
            anim = GetComponentInChildren<Animator>();
            weaponHolderManager = GetComponent<WeaponHolderManager>();
            animatorHook = GetComponentInChildren<AnimatorHook>();

            clothManager = GetComponent<ClothManager>();
            clothManager.Init();
            clothManager.LoadListOfItems(startingCloth);

            ResetCurrentActions();

            currentPosition = mTransform.position;
            ignoreForGroundCheck = ~(1 << 8 | 1 << 9 | 1 << 10 | 1 << 11);
            
            if (GeneralStatusController.easyMode)
            {
                this.health = 400;
            }
        }

        private void Update()
        {
            isInteracting = anim.GetBool("isInteracting");
            if (animatorHook.canDoCombo)
            {
                if (!isInteracting)
                {
                    animatorHook.canDoCombo = false;
                }
            }

            if (hitTimer > 0)
            {
                hitTimer -= Time.deltaTime;
                if(hitTimer <= 0)
                {
                    isHit = false;
                }
            }
        }

        private void FixedUpdate()
        {
            if (!isDead)
            {
                if (health <= 0)
                {
                    isDead = true;
                    PlayTargetAnimation("Death", true, false);
                    deathSound.Play();
                    return;
                } 
            }
        }

        #region Movement
        public void HandleCombo() { }

        Vector3 currentPosition;

        public void MoveCharacter(float vertical, float horizontal, Vector3 movementDirection, float delta)
        {
            CheckGround();
            float moveAmount = Mathf.Clamp01(Mathf.Abs(horizontal) + Mathf.Abs(vertical));

            HandleDamageCollider();

            // HANDLE ROTATION
            if (!isInteracting || animatorHook.canRotate)
            {
                Vector3 rotationDir = movementDirection;

                if (lockOn && !isSprinting)
                {
                    rotationDir = currentLockTarget.position - mTransform.position;
                }

                HandleRotation (moveAmount, rotationDir, delta);
            }

            Vector3 targetVelocity = Vector3.zero;

            if (lockOn && !isSprinting)
            {
                targetVelocity = mTransform.forward * vertical * movementSpeed;
                targetVelocity += mTransform.right * horizontal * movementSpeed;
            }
            else
            {
                float speed = movementSpeed;
                if (isSprinting)
                {
                    speed = sprintSpeed;
                }
                
                targetVelocity = movementDirection * speed;
            }

            if (isInteracting)
            {
                targetVelocity = animatorHook.deltaPosition * velocityMultiplier;
            }

            // HANDLE MOVEMENT
            if (isGrounded)
            {
                targetVelocity = Vector3.ProjectOnPlane(targetVelocity, currentNormal);
                rigidbody.velocity = targetVelocity;

                Vector3 groundedPosition = mTransform.position;
                groundedPosition.y = currentPosition.y;
                mTransform.position = Vector3.Lerp(mTransform.position, groundedPosition, delta / groundedSpeed);
            }

            HandleAnimations (vertical, horizontal, moveAmount);
        }

        void CheckGround()
        {
            RaycastHit hit;
            Vector3 origin = mTransform.position;
            origin.y += .5f;

            float dis = goundedDistanceRay;
            if (isOnAir)
            {
                dis = groundDownDistanceOnAir;
            }

            Debug.DrawRay(origin, Vector3.down * dis, Color.red);
            if (Physics.SphereCast(origin, .2f, Vector3.down, out hit, dis, ignoreForGroundCheck))
            {
                isGrounded = true;
                currentPosition = hit.point;
                currentNormal = hit.normal;

                float angle = Vector3.Angle(Vector3.up, currentNormal);
                if(angle > 75){
                    isGrounded = false;
                }

                if (isOnAir)
                {
                    isOnAir = false;
                    PlayTargetAnimation("Empty", false, false);
                }
            }
            else
            {
                if (isGrounded)
                {
                    isGrounded = false;
                }

                if (!isOnAir)
                {
                    isOnAir = true;
                    PlayTargetAnimation("OnAir", true, false);
                }
            }
        }

        void HandleDamageCollider()
        {
            if(currentAction != null)
            {
                if(currentAction.weaponHook != null)
                {
                    currentAction.weaponHook.DamageColliderStatus(animatorHook.openDamageCollider);
                }
            }
        }

        void HandleRotation(float moveAmount, Vector3 targetDir, float delta)
        {
            float moveOverride = moveAmount;
            if (lockOn)
            {
                moveOverride = 1;
            }
            targetDir.Normalize();
            targetDir.y = 0;
            if (targetDir == Vector3.zero)
            {
                targetDir = mTransform.forward;
            }
            float actualRotationSpeed = rotationSpeed;
            if (isInteracting)
            {
                actualRotationSpeed = attackRotationSpeed;
            }
            Quaternion tr = Quaternion.LookRotation(targetDir);
            Quaternion targetRotation =
                Quaternion
                    .Slerp(mTransform.rotation,
                    tr,
                    delta * moveOverride * actualRotationSpeed);
            mTransform.rotation = targetRotation;
        }

        public void HandleAnimations(float vertical, float horizontal, float moveAmount)
        {
            if (isGrounded)
            {
                anim.SetBool("isSprinting", isSprinting);
                
                if (lockOn && !isSprinting)
                {
                    float v = Mathf.Abs(vertical);
                    float f = 0;
                    if (v > 0 && v < .5f)
                    {
                        f = .5f;
                    }
                    else if (v > 0.5f)
                    {
                        f = 1;
                    }
                    if (vertical < 0)
                    {
                        f = -f;
                    }
                    anim.SetFloat("Forward", f); //, .2f, Time.deltaTime);
                    float h = Mathf.Abs(horizontal);
                    float s = 0;
                    if (h > 0 && h < .5f)
                    {
                        s = .5f;
                    }
                    else if (h > 0.5f)
                    {
                        s = 1;
                    }
                    if (horizontal < 0)
                    {
                        s = -1;
                    }
                    anim.SetFloat("Sideway", s); //, .2f, Time.deltaTime);
                }
                else
                {
                    float m = moveAmount;
                    float f = 0;
                    if (m > 0 && m < .5f)
                    {
                        f = .5f;
                    }
                    else if (m > 0.5f)
                    {
                        f = 1;
                    }
                    anim.SetFloat("Forward", f, .2f, Time.deltaTime);
                    anim.SetFloat("Sideway", 0); //, 0.2f, Time.deltaTime);
                }
            }
        }

        #endregion
        
        #region Items & Actions

        void ResetCurrentActions()
        {
            currentActions = new ItemActionContainer[defaultActions.Length];
            for (int i = 0; i < defaultActions.Length; i++)
            {
                currentActions[i] = new ItemActionContainer();
                currentActions[i].animName = defaultActions[i].animName;
                currentActions[i].attackInput = defaultActions[i].attackInput;
                currentActions[i].isMirrored = defaultActions[i].isMirrored;
                currentActions[i].itemAction = defaultActions[i].itemAction;
                currentActions[i].itemActual = defaultActions[i].itemActual;
                currentActions[i].canBackstab = defaultActions[i].canBackstab;
                currentActions[i].canParry = defaultActions[i].canParry;
            }
        }

        public void PlayTargetAnimation(string targetAnimation, bool isInteracting, bool isMirror = false, float velocityMultiplier = 1)
        {
            anim.SetBool("isMirror", isMirror);
            anim.SetBool("isInteracting", isInteracting);
            anim.CrossFade(targetAnimation, 0.2f);
            this.isInteracting = isInteracting;
            this.velocityMultiplier = velocityMultiplier;
        }

        public void PlayTargetItemAction(AttackInputs attackInput)
        {
            animatorHook.canRotate = false;
            currentAction = GetItemActionContainer(attackInput, currentActions);

            if(currentAction.canBackstab || currentAction.canParry)
            {
                RaycastHit hit;
                Vector3 origin = mTransform.position;
                origin.y += 1;
                Vector3 dir = mTransform.forward;
                Debug.DrawRay(origin, dir * 2, Color.red, 2, false);
                if (Physics.SphereCast(origin, .5f, dir, out hit, 2))
                {
                    if (hit.collider.CompareTag("Enemy"))
                    {
                        IParryable parryable = hit.transform.GetComponentInParent<IParryable>();
                        if(parryable != null)
                        {
                            Transform enTransform = parryable.GetTransform();
                            
                            if (parryable.CanBeParried())
                            {
                                float angle = Vector3.Angle(-enTransform.forward, mTransform.forward);
                                if (angle < 45)
                                {
                                    PlayTargetAnimation("Parry Attack", true, currentAction.isMirrored);
                                    parryable.GetParried(mTransform.position, mTransform.forward);
                                    return;
                                }
                            }
                            else if(parryable.CanBeBackstabbed())
                            {
                                float angle = Vector3.Angle(enTransform.forward, mTransform.forward);
                                if (angle < 45)
                                {
                                    PlayTargetAnimation("Parry Attack", true, currentAction.isMirrored);
                                    parryable.GetBackstabbed(mTransform.position, mTransform.forward);
                                    return;
                                }
                            }
                        }
                    }
                }
            }
                

            if (!string.IsNullOrEmpty(currentAction.animName))
            {
                currentAction.ExecuteItemAction(this);
            }
        }

        protected ItemActionContainer GetItemActionContainer(AttackInputs ai, ItemActionContainer[] l)
        {
            if (l == null)
            {
                return null;
            }

            for (int i = 0; i < l.Length; i++)
            {
                if (l[i].attackInput == ai)
                {
                    return l[i];
                }
            }
            return null;
        }

        public void LoadWeapon(Item item, bool isLeft)
        {
            if (item is not WeaponItem)
            {
                return;
            }

            WeaponItem weaponItem = (WeaponItem) item;

            WeaponHook weaponHook = weaponHolderManager.LoadWeaponOnHook(weaponItem, isLeft);

            if (weaponItem == null)
            {
                ItemActionContainer da = GetItemActionContainer(GetAttackInput(AttackInputs.rt, isLeft), defaultActions);
                ItemActionContainer ta = GetItemActionContainer(GetAttackInput(AttackInputs.rt, isLeft), currentActions);
                CopyItemActionContainer (da, ta);
                ta.isMirrored = isLeft;
                ta.weaponHook = weaponHook;
                return;
            }

            for (int i = 0; i < weaponItem.itemActions.Length; i++)
            {
                ItemActionContainer wa = weaponItem.itemActions[i];
                ItemActionContainer ic = GetItemActionContainer(GetAttackInput(wa.attackInput, isLeft), currentActions);
                if(ic != null)
                {
                    CopyItemActionContainer(wa, ic);
                    ic.isMirrored = isLeft;
                    ic.weaponHook = weaponHook;
                }
            }
        }

        public void LoadCloth(Item item)
        {
            if(item is ClothItem)
            {
                clothManager.LoadItem((ClothItem)item);
            }
        }

        void CopyItemActionContainer(ItemActionContainer source, ItemActionContainer target)
        {
            target.animName = source.animName;
            target.itemAction = source.itemAction;
            target.itemActual = source.itemActual;
            target.canBackstab = source.canBackstab;
            target.canParry = source.canParry;
        }

        AttackInputs GetAttackInput(AttackInputs inp, bool isLeft)
        {
            if (!isLeft)
            {
                return inp;
            }
            else
            {
                switch (inp)
                {
                    case AttackInputs.rt:
                        return AttackInputs.lt;
                    case AttackInputs.lt:
                        return AttackInputs.rt;
                    default:
                        return inp;
                }
            }
        }

        #region Combos

        private Combo[] combos;

        public void LoadCombos(Combo[] targetCombo)
        {
            combos = targetCombo;
        }

        public void DoCombo(AttackInputs inp)
        {
            Combo c = GetComboFromInp(inp);
            if (c == null)
            {
                return;
            }
            animatorHook.canDoCombo = false;
            PlayTargetAnimation(c.animName, true, false);
        }

        Combo GetComboFromInp(AttackInputs inp)
        {
            if (combos == null)
            {
                return null;
            }

            for (int i = 0; i < combos.Length; i++)
            {
                if (combos[i].inp == inp)
                {
                    return combos[i];
                }
            }
            return null;
        }
        #endregion

        public ILockable FindLockableTarget(){
            Collider[] cols = Physics.OverlapSphere(mTransform.position, 20);
            ILockable minDist = null;
            float min = 20f;
            for(int i = 0; i < cols.Length; i++){
                ILockable lockable = cols[i].GetComponentInParent<ILockable>();
                if(lockable != null){
                    float dist = Vector3.Distance(mTransform.position, lockable.GetLockOnTarget(mTransform).position);
                    if (dist < min)
                    {
                        min = dist;
                        minDist = lockable;
                    }
                }
            }

            return minDist;
        }

        public ILockable FindLockableTarget(ILockable notLockTo)
        {
            Collider[] cols = Physics.OverlapSphere(mTransform.position, 20);
            ILockable minDist = null;
            float min = 20f;
            for (int i = 0; i < cols.Length; i++)
            {
                ILockable lockable = cols[i].GetComponentInParent<ILockable>();
                if(lockable != notLockTo)
                { 
                    if (lockable != null)
                    {
                        float dist = Vector3.Distance(mTransform.position, lockable.GetLockOnTarget(mTransform).position);
                        if (dist < min)
                        {
                            min = dist;
                            minDist = lockable;
                        }
                    }
                }
            }

            return minDist;
        }

        public ActionContainer GetActionContainer()
        {
            return LastAction;
        }

        bool isHit;
        float hitTimer;

        public void OnDamage(ActionContainer action)
        {
            generalStatusController ??= GeneralStatusController.singleton;
            if (generalStatusController.isImortal)
            {
                return;
            }
            
            if (action.owner == mTransform)
            {
                return;
            }
            
            if (!isHit)
            {
                animatorHook.openDamageCollider = false;
                isHit = true;
                hitTimer = 1;
                anim.SetBool("isDancing", false);
                manualDancing = false;
                if (GeneralStatusController.easyMode)
                {
                    health -= Mathf.FloorToInt(action.damage * 0.75f);
                }else
                {
                    health -= action.damage;
                }
                hitSound.Play();

                Vector3 dir = action.owner.position - mTransform.position;
                float dot = Vector3.Dot(dir, mTransform.forward);
                if (action.overrideReactAnim)
                {
                    PlayTargetAnimation(action.reactAnim, true);
                }
                else
                {
                    if (dot > 0)
                    {
                        PlayTargetAnimation("HitFront", true);
                    }
                    else
                    {
                        PlayTargetAnimation("HitBack", true);
                    }
                }
            }

        }

        #endregion

        public void Dance()
        {
            anim.SetBool("isDancing", isDancing);
            if (isDancing)
            {
                PlayTargetAnimation("Dance", true);
            }
            /*else
            {
                PlayTargetAnimation("Empty", true);
            }*/
        }
        int parryFrameCount;

        public void OpenParryCollider()
        {
            parryFrameCount = 0;
            parryCollider.SetActive(true);
        }

        void LateUpdate()
        {
            if(parryCollider.activeInHierarchy)
            {
                parryFrameCount++;
                if (parryFrameCount > 3)
                {
                    parryCollider.SetActive(false);
                }
            }
        }
        public Transform GetTransform()
        {
            return mTransform;
        }

        public void PlayAttackSound()
        {
            attackSound.Play();
        }
        
        public void OnParried(Vector3 dir) { }
        
        public void GetParried(Vector3 origin, Vector3 direction) { }

        public bool CanBeParried() { return false; }

        public bool CanBeBackstabbed() { return false; }

        public void GetBackstabbed(Vector3 origin, Vector3 direction) { }
    }

    [System.Serializable]
    public class Combo
    {
        public string animName;
        public AttackInputs inp;
    }
}
