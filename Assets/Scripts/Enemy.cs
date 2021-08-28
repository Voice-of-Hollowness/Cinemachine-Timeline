
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using Vector3 = UnityEngine.Vector3;

namespace Assets.Scripts
{
    [RequireComponent(typeof(NavMeshAgent))]

    

    public class Enemy : LivingEntetity
    {
        public enum State {Idle,Chasing,Attacking };
        public float damage=2;

        State curState;
        LivingEntetity targetEntetity;
        NavMeshAgent pathfinder;
        Transform target;
        Material skinMaterial;
        Color originalColor;

        public ParticleSystem deathEfect;
        
        private float AtkDist =0.5f;
        private float TBtweenAtk = 1;

        private float nexAtkTime;
        private float MyCollRadius;
        private float trgtCollRadius;
        private bool hasTarget;

        public static event System.Action OnDeathStatic;

        void Awake() {
            pathfinder = GetComponent<NavMeshAgent>();


            if (GameObject.FindGameObjectWithTag("Player") != null)
            {
                target = GameObject.FindGameObjectWithTag("Player").transform;
               
                hasTarget = true;

                MyCollRadius = GetComponent<CapsuleCollider>().radius;
                trgtCollRadius = target.GetComponent<CapsuleCollider>().radius;
                targetEntetity = target.GetComponent<LivingEntetity>();
 
            }

        }

        protected void Start()
        {
            var bullet = GetComponent<Projectile>();
            base.Start();





            if (hasTarget)
            {
                curState = State.Chasing;
 

                StartCoroutine(UpdatePath());


                targetEntetity.OnDeath += OnTargetDeath;
            }

        }

        void OnTargetDeath()
        {
            hasTarget = false;
            curState = State.Idle;

        }

        void Update()
        {
            if(hasTarget)
            {
                if (Time.time > nexAtkTime)
                {
                    float Dist2Target = (target.position - transform.position).sqrMagnitude;

                    if (Dist2Target < Mathf.Pow(AtkDist + MyCollRadius + trgtCollRadius, 2))
                    {
                        if (curState == State.Chasing)
                        {
                            nexAtkTime = Time.time + TBtweenAtk;
                            AudioManager.instance.PlaySound("EnemyAttack", transform.position);
                            StartCoroutine(Attack());

                        }
                    }
                }
            }
        }

        public override void TakeHit(float damage, Vector3 hitPoint, Vector3 hitDirect)
        {
            AudioManager.instance.PlaySound("Impact",transform.position);

            if (damage >= health) {
                if (OnDeathStatic!=null)
                {
                    OnDeathStatic();
                }
                Destroy( Instantiate(deathEfect.gameObject, hitPoint, Quaternion.FromToRotation(Vector3.forward,hitDirect))as GameObject,deathEfect.startLifetime);
            }
            base.TakeHit(damage, hitPoint, hitDirect);
        }

        public void SetCharacteristics(int Hp, float Spd, int Atk, Color Skin)
        {
            pathfinder.speed = Spd;
            if (hasTarget)
            {
                damage = Mathf.Ceil( targetEntetity.startHealth / Atk);
            }
            
            
            startHealth = Hp;
            deathEfect.startColor = new Color(Skin.r,Skin.g,Skin.b,1);
            skinMaterial = GetComponent<Renderer>().material;
            skinMaterial.color = Skin;
            originalColor = skinMaterial.color;
        }



        private bool _isAttacking;
        IEnumerator Attack()
        {
            _isAttacking = true;

            pathfinder.enabled = false;
            curState = State.Attacking;
            Vector3 origPos = transform.position;
            Vector3 dirToTrgt = (target.position - transform.position).normalized;
            Vector3 AtkPos = target.position - dirToTrgt * (MyCollRadius + trgtCollRadius + AtkDist / 2);

            float atkSpeed = 3;
            float percent = 0;

            bool hasApplyDmg = false;

                skinMaterial.color=Color.yellow;

            while (percent <= 1)
            {
                if (percent >=0.5f && hasApplyDmg ==false)
                {
                    hasApplyDmg = true;
                    targetEntetity.TakeDamage(damage);
                }
                percent += Time.deltaTime * atkSpeed;
                float interpolation = (-Mathf.Pow(percent,2)+ percent) * 4;
                transform.position = Vector3.Lerp(origPos,AtkPos,interpolation);

                yield return null;
            }

            _isAttacking = false;
            curState = State.Chasing;
            pathfinder.enabled = true;
            skinMaterial.color = originalColor;
        }

        IEnumerator UpdatePath()
        {
            float refreshRate = .25f;

          
            while (hasTarget)
            {
 
                if (!dead&&!_isAttacking)
                {
                    Vector3 dirToTrgt = (target.position - transform.position).normalized;

                    Vector3 targetPosition = target.position - dirToTrgt * (MyCollRadius);
                    if (pathfinder == null)
                        break;

                        pathfinder.SetDestination(targetPosition);
                }

                yield return new WaitForSeconds(refreshRate);
            }
        }



    }

}
