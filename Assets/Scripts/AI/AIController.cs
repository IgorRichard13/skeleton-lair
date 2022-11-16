using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

namespace R2
{
    public class AIController : MonoBehaviour, ILockable, IDamageable, IHaveAction, IParryable
    {
        new Rigidbody rigidbody;
        Animator animator;
        NavMeshAgent agent;
        AnimatorHook animatorHook;
        Transform mTransform;
        GeneralStatusController generalStatusController;

        [Header("Boss Settings")]
        public GameObject bulletPrefab;
        public bool isBoss;
        public bool firstAttack2 = false;
        public bool firstAttack3 = false;
        public GameObject enemyPrefab;

        [Header("General Settings")]
        public int health = 100;

        public float fovRadius = 20;
        public float rotationSpeed = 1;
        public float moveSpeed = 1;
        public float recoveryTimer;
        public bool isInInterruption;
        public bool openToBackstab = true;
        LayerMask detectionLayer;

        public int hardAction = -1;

        public Controller currentTarget;
        bool isInteracting = false;
        bool actionFlag = false;
        public ActionSnapshot[] actionSnapshots;
        ActionSnapshot currentSnapshot;
        
        ActionContainer _lastAction;
        public ActionContainer LastAction
        {
            get
            { 
                _lastAction ??= new ActionContainer();

                if (currentSnapshot == null)
                {
                    _lastAction.owner = mTransform;
                    _lastAction.damage = 0;
                    _lastAction.overrideReactAnim = false;
                    _lastAction.reactAnim = "";
                }
                else
                {
                    _lastAction.owner = mTransform;
                    _lastAction.damage = currentSnapshot.damage;
                    _lastAction.overrideReactAnim = currentSnapshot.overrideReactAnim;
                    _lastAction.reactAnim = currentSnapshot.reactAnim;
                }
                
                return _lastAction;
            }
        }

        public ActionSnapshot GetAction(float distance, float angle)
        {
            int h1 = 250;
            int h2 = 750;
            if(GeneralStatusController.easyMode)
            {
                h1 /= 2;
                h2 /= 2;
            }
            if (isBoss)
            {
                if (health < h1)
                {
                    if (!firstAttack3)
                    {
                        hardAction = 2;
                        firstAttack3 = true;
                    }
                }
                
                if (health >= h1 && health < h2)
                {
                    if(!firstAttack2)
                    {
                        hardAction = 1;
                        firstAttack2 = true;
                    }
                }
                
                if (health >= h2)
                {
                    hardAction = 0;
                }
            }

            if (hardAction != -1)
            {
                int tmp = hardAction;
                hardAction = -1;
                return actionSnapshots[tmp];
            }

            int maxScore = 0;
            for (int i = 0; i < actionSnapshots.Length; i++)
            {
                ActionSnapshot a = actionSnapshots[i];
                if (distance <= a.maxDist && distance >= a.minDist)
                {
                    if (angle <= a.maxAngle && angle >= a.minAngle)
                    {
                        maxScore += a.score;
                    }
                }
            }

            int ran = Random.Range(0, maxScore + 1);
            int temp = 0;

            for (int i = 0; i < actionSnapshots.Length; i++)
            {
                ActionSnapshot a = actionSnapshots[i];
                
                if (a.score == 0)
                {
                    continue;
                }

                if (distance <= a.maxDist && distance >= a.minDist)
                {
                    if (angle <= a.maxAngle && angle >= a.minAngle)
                    {
                        temp += a.score;
                        if (temp >= ran)
                        {
                            if(isBoss)
                            {
                                if(health >= h1)
                                {
                                    if(i == 3)
                                    {
                                        continue;
                                    }
                                }

                                if (health >= h2)
                                {
                                    if (i == 2)
                                    {
                                        continue;
                                    }
                                }
                            }
                            
                            return a;
                        }
                    }
                }
            }

            return null;
        }

        /* Para esquema do escudo, pegar um getcomponentinparent
         * em algum script no escudo, e obter esse aiController aq
         * Então fazer detectar se o collider da espada bateu no escudo antes de bater na hitbox
         * caso sim, fazer o escudo bloquear a espada, rodando a animação
         * e colocando uma ivencibilidade no inimigo por um tempo.
         * 
         * Ideia de fazer ele ficar sempre em alguma posição de bloqueio enquanto
         * estiver no recoveryTimer.
         */

        /* Para o boss, no ep 183 de unity do sharp accent, ele mostra
         * alguma técnicas de damageColliders, pode ser interessante pra pegar alguma ideia
         */

        private void Start()
        {
            detectionLayer = (1 << 8);
            mTransform = this.transform;
            rigidbody = GetComponentInChildren<Rigidbody>();
            animator = GetComponentInChildren<Animator>();
            agent = GetComponentInChildren<NavMeshAgent>();
            animatorHook = GetComponentInChildren<AnimatorHook>();
            rigidbody.isKinematic = false;
            agent.stoppingDistance = 2.5f;
            generalStatusController = GeneralStatusController.singleton;
            if (GeneralStatusController.easyMode)
            {
                health /= 2;
            }
        }

        private void Update(){
            float delta = Time.deltaTime;

            isInInterruption = animator.GetBool("interrupted");
            isInteracting = animator.GetBool("isInteracting");

            if(isHit)
            {
                if (hitTimer > 0)
                {
                    hitTimer -= delta;
                }
                else
                {
                    isHit = false;
                }
            }
            
            if(currentTarget == null)
            {
                HandleDetection();
            }
            else
            {
                if (agent.isActiveAndEnabled)
                {
                    agent.SetDestination(currentTarget.mTransform.position);
                }

                Vector3 relativeDirection = mTransform.InverseTransformDirection(agent.desiredVelocity);

                if (!isInteracting)
                {
                    if(actionFlag)
                    {
                        recoveryTimer -= delta;
                        if (recoveryTimer <= 0)
                        {
                            actionFlag = false;
                        }
                    }

                    animator.SetFloat("movement", relativeDirection.z, 0.1f, delta);
                    
                    Vector3 dir = currentTarget.mTransform.position - mTransform.position;
                    dir.y = 0;
                    dir.Normalize();
                    
                    float dis = Vector3.Distance(mTransform.position, currentTarget.mTransform.position);
                    float angle = Vector3.Angle(mTransform.forward, dir);
                    float dot = Vector3.Dot(mTransform.right, dir);
                    if (dot < 0)
                    {
                        angle = -angle;
                    }

                    currentSnapshot = GetAction(dis, angle);
                    if (currentSnapshot != null && !actionFlag)
                    {
                        PlayTargetAnimation(currentSnapshot.anim, true);
                        actionFlag = true;
                        recoveryTimer = currentSnapshot.recoveryTime;
                    }
                    else
                    {
                        animator.SetFloat("sideways", relativeDirection.x, 0.1f, delta);
                    }
                }

                if (!isInteracting)
                {
                    agent.enabled = true;
                    mTransform.rotation = agent.transform.rotation;
                    Vector3 lookPosition = currentTarget.mTransform.position;
                    lookPosition.y += 1.2f;
                    animatorHook.lookAtPosition = lookPosition;
                }

                if(isInteracting)
                {
                    if(animatorHook.canRotate)
                    {
                        HandleRotation(delta);
                    }
                    agent.enabled = false;
                    animator.SetFloat("movement", 0, 0.1f, delta);
                    animator.SetFloat("sideways", 0, 0.1f, delta);
                    if(currentSnapshot != null)
                    {
                        currentSnapshot.damageCollider.SetActive(animatorHook.openDamageCollider);
                    }
                }

                Vector3 targetVel = animatorHook.deltaPosition * moveSpeed;
                rigidbody.velocity = targetVel;
            }
        }

        private void LateUpdate()
        {
            agent.transform.localPosition = Vector3.zero;
            agent.transform.localRotation = Quaternion.identity;
        }

        public void PlayTargetAnimation(string targetAnimation, bool isInteracting, float crossFadeTime = 0.2f, bool playInstantly = false)
        {
            animator.SetBool("isInteracting", isInteracting);
            
            if (!playInstantly)
            {
                animator.CrossFade(targetAnimation, crossFadeTime);
            }
            else
            {
                animator.Play(targetAnimation);
            }
        }

        void HandleRotation(float delta)
        {
            Vector3 dir = currentTarget.mTransform.position - mTransform.position;
            dir.y=0;
            dir.Normalize();

            if(dir == Vector3.zero){
                dir = mTransform.forward;
            }

            float angle = Vector3.Angle(dir, mTransform.forward);
            if(angle > 5)
            {
                animator.SetFloat("sideways", Vector3.Dot(dir, mTransform.right), 0.1f, delta);
            }
            else
            {
                animator.SetFloat("sideways", 0, 0.1f, delta);
            }

            Quaternion targetRotation = Quaternion.LookRotation(dir);
            mTransform.rotation = Quaternion.Slerp(mTransform.rotation, targetRotation, delta / rotationSpeed);
        }

        void HandleDetection()
        {
            Collider[] cols = Physics.OverlapSphere(mTransform.position, fovRadius, detectionLayer);

            for(int i = 0; i < cols.Length; i++){
                Controller controller = cols[i].transform.GetComponentInParent<Controller>();
                if(controller != null){
                    currentTarget =  controller;
                    animatorHook.hasLookAtTarget = true;
                    return;
                }
            }
        }

        public Transform lockOnTarget;
        public Transform GetLockOnTarget(Transform from)
        {
            return lockOnTarget;
        }

        bool isHit;
        float hitTimer;
        
        public void OnDamage(ActionContainer action)
        {
            if (action.owner.gameObject.CompareTag(this.gameObject.tag) || action.owner == mTransform)
            {
                return;
            }

            if (!isHit)
            {
                animatorHook.openDamageCollider = false;
                isHit = true;
                hitTimer = 0.5f;
                health -= action.damage;
                if(!isBoss)
                {
                    EnemySounds.singleton.hitSound.Play();
                }
                else
                {
                    EnemySounds.singleton.BossHitSound.Play();
                }

                GameObject blood = R2.Utilities.ObjectPooler.GetObject("bloodFx");
                blood.transform.position = mTransform.position + Vector3.up * 1.5f;
                blood.transform.rotation = mTransform.rotation;
                blood.SetActive(true);

                if (health <= 0)
                {
                    Die();
                }
                else
                {
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
                            PlayTargetAnimation("Damage 1", true);
                        }
                        else
                        {
                            PlayTargetAnimation("Damage 2", true);
                        }
                    }
                }
            }
        }

        public bool IsAlive()
        {
            return health > 0;
        }

        public ActionContainer GetActionContainer()
        {
            return LastAction;
        }

        public void OnParried(Vector3 dir)
        {
            if (animatorHook.canBeParried)
            {
                if(!isInInterruption)
                {
                    animatorHook.openDamageCollider = false;
                    dir.Normalize();
                    dir.y = 0;
                    mTransform.rotation = Quaternion.LookRotation(dir);
                    PlayTargetAnimation("Attack Interrupt", true);
                }
            }
        }

        public Transform GetTransform()
        {
            return mTransform;
        }

        public float parriedDistance = 1.5f;

        public void GetParried(Vector3 origin, Vector3 direction)
        {
            mTransform.position = origin + direction * parriedDistance;
            mTransform.rotation = Quaternion.LookRotation(-direction);
            PlayTargetAnimation("Getting Parried", true, 0, true);
            health -= 50;
        }

        public bool CanBeParried()
        {
            return isInInterruption;
        }

        public bool CanBeBackstabbed()
        {
            return openToBackstab;
        }

        public void GetBackstabbed(Vector3 origin, Vector3 direction)
        {
            mTransform.position = origin + direction * parriedDistance;
            mTransform.rotation = Quaternion.LookRotation(direction);
            PlayTargetAnimation("Getting Backstabbed", true, 0, true);
        }

        internal void BossShoot(int id)
        {
            GameObject spawn = null;
            if (id == 1 )
            {
                spawn = GameObject.Find("Lord_Infandum_RPalm");
            }
            else if (id == 2)
            {
                spawn = GameObject.Find("Lord_Infandum_LPalm");
            }
            var projectile = Instantiate(bulletPrefab, spawn.transform.position, spawn.transform.rotation);
            projectile.GetComponent<ShootCollider>().currentTarget = currentTarget;
            projectile.GetComponent<ShootCollider>().Direction();
        }

        public void PlayAttackSound()
        {
            if(isBoss)
            {
                EnemySounds.singleton.BossAttackSound.Play();
            }
            else
            { 
                EnemySounds.singleton.attackSound.Play();
            }
        }

        internal void SpawnEnemies()
        {
            //Find all tags SpawnPointEnemy
            GameObject[] spawnPoints = GameObject.FindGameObjectsWithTag("BossSpawnPoint");
            if(isBoss)
            {
                for(int i = 0; i < 5; i++)
                {
                    int rand = Random.Range(0, spawnPoints.Length-1);
                    GameObject enemy = Instantiate(enemyPrefab, spawnPoints[rand].transform.position, spawnPoints[rand].transform.rotation);
                    enemy.GetComponent<AIController>().isBoss = false;
                    enemy.GetComponent<AIController>().currentTarget = currentTarget;
                }
            }
        }

        public void Die()
        {
            if (!isBoss)
            {
                EnemySounds.singleton.deathSound.Play();
            }
            else
            {
                EnemySounds.singleton.BossDeathSound.Play();
                LevelManager.singleton.LoadScene(3);
            }
            #region RandDeathAnim
            float rand = Random.Range(0, 2.5f);
            if (rand <= 1.24f)
            {
                PlayTargetAnimation("Death 1", true);
            }
            else
            {
                PlayTargetAnimation("Death 2", true);
            }
            #endregion
            animator.transform.parent = null;
            gameObject.SetActive(false);
            generalStatusController.killedSkeletonsCount++;
            if (isBoss)
            {
                /* TODO */
                Debug.Log("chama uma cutscene ou algo assim");
            }
        }
    }

    [System.Serializable]
    public class ActionSnapshot
    {
        public string anim;
        public int score = 5;
        public float recoveryTime;
        public float minAngle = -45;
        public float maxAngle = 45;
        public float minDist = 2;
        public float maxDist = 5;
        public GameObject damageCollider;
        public int damage = 20;
        public bool overrideReactAnim;
        public string reactAnim;
    }
}
