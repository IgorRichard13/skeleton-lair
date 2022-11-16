using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

namespace R2
{
    public class InputManager : MonoBehaviour
    {
        public CameraManager cameraManager;
        public Controller controller;
        public Transform camTransform;

        [HideInInspector]
        public AIController lockOnEnemy;
        PauseMenuScreen pauseMenuScreen;

        public GameObject pauseMenu;
        public GameObject debugMenu;
        public AudioSource fortnite;
        
        bool isAttacking, retVal;
        bool rb, rt, lb, lt;
        bool start, select;
        bool leftStickClick, rightStickClick;
        bool dpadUP, dpadDOWN, dpadLEFT, dpadRIGHT;
        bool interact, lockOnInput, rollInput, b_button;

        float vertical;
        float horizontal;
        float moveAmount;
        float mouseX;
        float mouseY;
        bool rollFlag;
        float rollTimer;
        float lockOnTimer;
        float danceTimer = 0;
        float inactivityTimer = 0;
        float startTimer = 0;
        float healthTimer = 0;
        float debugTimer = 0;
        bool lockOnFlag = false;

        Vector2 moveDirection;
        Vector2 cameraDirection;
        public PlayerControls keys;

        ILockable currentLockable;

        public PlayerProfile playerProfile;

        public ExecutionOrder cameraMovement;

        GeneralStatusController generalStatusController;

        public enum ExecutionOrder{
            fixedUpdate, update, lateUpdate
        }

        private void Start(){
            
            ResourcesManager rm = Settings.resourcesManager;
            for (int i = 0; i < playerProfile.startingClothes.Length; i++)
            {
                Item item = rm.GetItem(playerProfile.startingClothes[i]);
                if (item is ClothItem)
                {
                    controller.startingCloth.Add((ClothItem)item);
                }
            }

            controller.Init();

            controller.SetWeapons(rm.GetItem(playerProfile.rightHandWeapon), rm.GetItem(playerProfile.leftHandWeapon));
            cameraManager.targetTransform = controller.transform;

            PlayerControls playerControls = new();
            #region NewInputManager
            keys = playerControls;
            keys.Enable();
            keys.Player.Movement.performed += i => moveDirection = i.ReadValue<Vector2>();
            keys.Player.Camera.performed += i => cameraDirection = i.ReadValue<Vector2>();
            #endregion

            Settings.interactionsLayer = (1 << 15);
            generalStatusController = GeneralStatusController.singleton;
            pauseMenuScreen = pauseMenu.GetComponent<PauseMenuScreen>();
        }

        private void OnDisable()
        {
            keys.Disable();
        }

        private void FixedUpdate(){
            if(controller == null){
                return;
            }

            float delta = Time.fixedDeltaTime;

            HandleMovement(delta);
            cameraManager.FollowTarget(delta);

            if(cameraMovement == ExecutionOrder.fixedUpdate){
                cameraManager.HandleRotation(delta, mouseX, mouseY);
            }
        }

        private void Update(){
            if(controller == null){
                return;
            }

            float delta = Time.deltaTime;
            
            if(lockOnFlag)
            {
                lockOnTimer -= delta;
                
                if (lockOnTimer <= 0)
                {
                    lockOnFlag = false;
                }
            }

            if(danceTimer > 0)
            {
                danceTimer -= delta;
            }

            if(startTimer > 0)
            {
                startTimer -= delta;
            }

            if (healthTimer > 0)
            {
                healthTimer -= delta;
            }

            if (debugTimer > 0)
            {
                debugTimer -= delta;
            }

            HandleInput();

            if(rollInput){
                rollFlag = true;
                rollTimer += delta;
            }

            if(cameraMovement == ExecutionOrder.update){
                cameraManager.HandleRotation(Time.deltaTime, mouseX, mouseY);
            }
        }

        private void LateUpdate(){
            if(controller == null){
                return;
            }

            if(cameraMovement == ExecutionOrder.lateUpdate){
                // cameraManager.FollowTarget(Time.deltaTime);
            }

            if(interact)
            {
                HandleInteractions();
            }

            if(controller.isDead)
            {
                OnDisable();
            }

            HandleInteractionsDetections();
        }

        void HandleMovement(float delta){
            Vector3 movementDirection = camTransform.right * horizontal;
            movementDirection += camTransform.forward * vertical;
            movementDirection.Normalize();

            controller.MoveCharacter(vertical, horizontal, movementDirection, delta);
        }

        bool GetButtonStatusPerformed(UnityEngine.InputSystem.InputActionPhase phase)
        {
            return phase == UnityEngine.InputSystem.InputActionPhase.Performed;
        }

        void HandleInput()
        {
            retVal = false;

            #region Input Detection

            bool isUsingMouse = false;
            if (generalStatusController.isDead || generalStatusController.isPaused)
            {
                isUsingMouse = true;
            }

            if(!(isUsingMouse || generalStatusController.isDebugging))
            {
                horizontal = moveDirection.x;
                vertical = moveDirection.y;

                mouseX = cameraDirection.x;
                mouseY = cameraDirection.y;

                rt = GetButtonStatusPerformed(keys.Player.RT.phase);
                rb = GetButtonStatusPerformed(keys.Player.RB.phase);
                lt = GetButtonStatusPerformed(keys.Player.LT.phase);
                lb = GetButtonStatusPerformed(keys.Player.LB.phase);

                select = GetButtonStatusPerformed(keys.Player.Select.phase);
                dpadUP = GetButtonStatusPerformed(keys.Player.DpadUp.phase);
                dpadDOWN = GetButtonStatusPerformed(keys.Player.DpadDown.phase);
                dpadLEFT = GetButtonStatusPerformed(keys.Player.DpadLeft.phase);
                dpadRIGHT = GetButtonStatusPerformed(keys.Player.DpadRight.phase);

                interact = GetButtonStatusPerformed(keys.Player.Interact.phase);
                rollInput = GetButtonStatusPerformed(keys.Player.Roll.phase);
                lockOnInput = GetButtonStatusPerformed(keys.Player.LockOn.phase);
                b_button = GetButtonStatusPerformed(keys.Player.B.phase);

                leftStickClick = GetButtonStatusPerformed(keys.Player.LeftStickClick.phase);
                rightStickClick = GetButtonStatusPerformed(keys.Player.RightStickClick.phase);
            }
            
            if(!generalStatusController.isDebugging)
            {
                start = GetButtonStatusPerformed(keys.Player.Start.phase);
            }
            #endregion

            if (!generalStatusController.isDead)
            {
                moveAmount = Mathf.Clamp01(Mathf.Abs(horizontal) + Mathf.Abs(vertical));
            }

            if (!controller.isInteracting){
                if(retVal == false){
                    retVal = HandleRolls();
                }
            }

            if(retVal == false){
                retVal = HandleAttacking();
            }

            if (lb && !controller.isInteracting)
            {
                controller.isSprinting = true;
            }
            else
            {
                controller.isSprinting = false;
            }

            if(!lockOnFlag)
            {
                if(lockOnInput){
                    lockOnFlag = true;
                    lockOnTimer = 1;
                    if(controller.lockOn){
                        DisableLockOn();
                    }
                    else{
                        Transform lockTarget = null;
                        currentLockable = controller.FindLockableTarget();
                        if(currentLockable != null)
                        {
                            lockTarget = currentLockable.GetLockOnTarget(controller.mTransform);
                        }
                        if(lockTarget != null){
                            cameraManager.lockTarget = lockTarget;
                            controller.lockOn = true;
                            controller.currentLockTarget = lockTarget;
                        }else{
                            cameraManager.lockTarget = null;
                            controller.lockOn = false;
                        }
                    }
                }

                if(rightStickClick && controller.lockOn)
                {
                    lockOnFlag = true;
                    lockOnTimer = 1;
                    currentLockable = controller.FindLockableTarget(currentLockable);
                    Transform lockTarget = null;
                    if (currentLockable != null)
                    {
                        lockTarget = currentLockable.GetLockOnTarget(controller.mTransform);
                    }
                    if (lockTarget != null)
                    {
                        cameraManager.lockTarget = lockTarget;
                        controller.lockOn = true;
                        controller.currentLockTarget = lockTarget;
                    }
                    else
                    {
                        cameraManager.lockTarget = null;
                        controller.lockOn = false;
                    }
                }
            }

            if(controller.lockOn)
            {
                if(!currentLockable.IsAlive())
                {
                    DisableLockOn();
                }
            }
            
            if (controller.manualDancing == false)
            {
                bool c;
                bool p;
                bool d;
                bool dd;

                c = DialogueManager.singleton.isChatting;
                p = GeneralStatusController.singleton.isPaused;
                d = GeneralStatusController.singleton.isDead;
                dd = GeneralStatusController.singleton.isDebugging;

                if (!(rb || rt || lb || lt ||
                    dpadUP || dpadDOWN || dpadLEFT || dpadRIGHT ||
                    leftStickClick || rightStickClick ||
                    start || select
                    || interact || rollInput || lockOnInput || b_button
                    || moveAmount > 0 || c || p || d || dd))
                {
                    inactivityTimer += Time.deltaTime;
                }
                else
                {
                    inactivityTimer = 0;
                    if (controller.isDancing)
                    {
                        controller.isDancing = false;
                        fortnite.Stop();
                        controller.Dance();
                    }
                }

                if (!controller.isDancing)
                {
                    if (inactivityTimer >= 15)
                    {

                        controller.isDancing = true;
                        fortnite.Play();
                        controller.Dance();
                    }
                }
            }

            if(select)
            {
                Cursor.lockState = CursorLockMode.Confined;
                Cursor.visible = true;
            }
            else
            {
                bool locked = false;
                if (!generalStatusController.isDead && !generalStatusController.isPaused)
                {
                    locked = true;
                }

                if(locked)
                {
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                }
                else
                {
                    Cursor.lockState = CursorLockMode.Confined;
                    Cursor.visible = true;
                }
            }

            if(start && startTimer <= 0)
            {
                Cursor.lockState = CursorLockMode.Confined;
                Cursor.visible = true;

                bool p = false;
                bool s = false;
                
                startTimer = 1;
                if (generalStatusController.isPaused)
                {
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                    generalStatusController.isPaused = false;
                    pauseMenu.SetActive(false);
                    s = true;
                }
                else
                {
                    generalStatusController.isPaused = true;
                    pauseMenu.SetActive(true);
                    p = true;
                }

                if(p)
                {
                    EventSystem.current.SetSelectedGameObject(null);
                    EventSystem.current.SetSelectedGameObject(GameUI.singleton.pauseFirstButton);
                }
            }

            if (dpadUP)
            {
                if (healthTimer <= 0)
                {
                    healthTimer = 2f;
                    generalStatusController.UsePotion();
                }
            }

            if (leftStickClick && rightStickClick)
            {
                if(debugTimer <= 0)
                {
                    debugTimer = 1f;
                    debugMenu.SetActive(true);
                    debugMenu.GetComponent<DebugMenu>().Initialize();
                    leftStickClick = false;
                    rightStickClick = false;
                }
                return;
            }
            
            if (!controller.lockOn)
            {
                if (rightStickClick)
                {
                    if (danceTimer <= 0)
                    {
                        danceTimer = 1;
                        if (controller.isDancing)
                        {
                            controller.isDancing = false;
                            controller.manualDancing = false;
                            fortnite.Stop();
                        }
                        else
                        {
                            controller.isDancing = true;
                            controller.manualDancing = true;
                            fortnite.Play();
                        }
                        controller.Dance();
                    }
                }
            }
        }

        void DisableLockOn()
        {
            cameraManager.lockTarget = null;
            controller.lockOn = false;
            controller.currentLockTarget = null;
            currentLockable = null;
        }

        bool HandleAttacking(){

            AttackInputs attackInput = AttackInputs.none;
            
            if(rt || lt){
                isAttacking = true;
                
                if(rt){
                    attackInput = AttackInputs.rt;
                }

                if(lt){
                    attackInput = AttackInputs.lt;
                }
            }

            if(interact){
                // controller.HandleTwoHanded();
            }

            if(attackInput != AttackInputs.none){
                if(!controller.isInteracting){
                    controller.PlayTargetItemAction(attackInput);
                }else{
                    if(controller.animatorHook.canDoCombo){
                        controller.DoCombo(attackInput);
                    }
                }
            }

            return isAttacking;
        }
        

        bool HandleRolls(){
            if(rollInput == false && rollFlag){
                rollFlag = false;

                if(moveAmount > 0){ //rollTimer > 0.5f
                    Vector3 movementDirection = camTransform.right * horizontal;
                    movementDirection += camTransform.forward * vertical;
                    movementDirection.Normalize();
                    movementDirection.y = 0;

                    Quaternion dir = Quaternion.LookRotation(movementDirection);
                    controller.transform.rotation = dir;
                    controller.PlayTargetAnimation("Roll", true, false, 1.5f);
                    return true;
                }else{
                    controller.PlayTargetAnimation("Step Back", true, false);
                }
            }
            return false;
        }

        IInteractable currentInteractable;

        void HandleInteractionsDetections()
        {
            GameUI gameUI = GameUI.singleton;
            currentInteractable = null;
            gameUI.ResetInteraction();
            
            Collider[] colliders = Physics.OverlapSphere(controller.mTransform.position, 1.5f, Settings.interactionsLayer);

            for (int i = 0; i < colliders.Length; i++)
            {
                IInteractable interactable = colliders[i].GetComponentInParent<IInteractable>();
                if(interactable != null)
                {
                    currentInteractable = interactable;
                    gameUI.LoadInteraction(interactable.GetInteractionType());
                    break;
                }
            }
        }

        void HandleInteractions()
        {
            if(currentInteractable != null)
            {
                currentInteractable.OnInteract(this);
            }
        }
    }
}
