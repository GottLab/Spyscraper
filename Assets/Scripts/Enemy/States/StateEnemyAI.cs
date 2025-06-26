using System;
using UnityEngine;
using UnityEngine.AI;

namespace Enemy
{
    [RequireComponent(typeof(EnemyAnimation)), RequireComponent(typeof(Animator))]
    public class StateEnemyAI : MonoBehaviour
    {
        private BehaviourStateMachine stateMachine;
        public BehaviourStateMachine StateMachine => stateMachine;


        [Header("Behaviour Settings")]

        [SerializeField]
        private QteSequence qteSequence;
        public QteSequence QteSequence => qteSequence;

        
        public float defaultSpeed = 2.0f;

        [SerializeField]
        private PatrolData patrolData;
        public PatrolData PatrolData => patrolData;


        [SerializeField]
        private AttackStateEnemy.AttackData attackData;
        public AttackStateEnemy.AttackData AttackData => attackData;

        private NavMeshAgent navMeshAgent;
        public NavMeshAgent NavMeshAgent => navMeshAgent;
        
        [SerializeField, Tooltip("Cone vision used by the enemy")]
        private ConeVision enemyVision;

        [SerializeField, Tooltip("Target of enemy vision that raises its suspition")]
        private ConeVisionObject visionTarget;

        [Header("Rendering")]


        [Tooltip("Default Mask Color")]
        public Color defaultMaskColor = Color.yellow;

        [NonSerialized]
        public EnemyAnimation enemyAnimation;

        [Tooltip("This transform position is used as target for the qte camera")]
        public Transform head;


        [SerializeField, Tooltip("Renderer used to render enemy, this is used to retrieve lens material to change its color")]
        private Renderer enemyRenderer;

        [Header("Suspition")]

        //SUSPITION VARIABLES


        [SerializeField, Tooltip("Time it takes in seconds to fill up suspition")]
        private float suspitionIncreaseTime = 1.0f;

        [SerializeField, Tooltip("Time it takes in seconds to empty suspition")]
        private float suspitionDecreaseTime = 1.0f;

        [SerializeField, Tooltip("How much time in seconds needs to pass before decreasing suspition")]
        private float suspitionAvoidanceTime = 1.0f;

         //suspition level of the enemy it goes from 0 to 100
        private float currentSuspition = 0.0f;

        //the last time the enemy increased it's suspition level
        private double lastTimeIncrease;

        //called when suspition level increases or decreases
        public event Action<bool> OnSuspitionChange;
        public event Action OnSuspitionReset;


        void Start()
        {
            stateMachine = new BehaviourStateMachine();
            navMeshAgent = GetComponent<NavMeshAgent>();
            stateMachine.SetState(new PatrolStateEnemy(this));
            enemyAnimation = GetComponent<EnemyAnimation>();
            this.UpdateVisionColor(this.defaultMaskColor);

            this.NavMeshAgent.speed = this.defaultSpeed;
        }

        void Update()
        {
            stateMachine.Update();
            UpdateSuspition();
        }



        void UpdateSuspition()
        {

            double timePassedFromPastSuspition = Time.timeAsDouble - this.lastTimeIncrease;
            if (this.visionTarget != null && this.stateMachine.CurrentState.CanAttackPlayer() && this.enemyVision.CheckVision(this.visionTarget))
            {
                UpdateSuspitionValue(true);
                this.lastTimeIncrease = Time.timeAsDouble;
            }
            else if (timePassedFromPastSuspition > suspitionAvoidanceTime)
            {
                UpdateSuspitionValue(false);
            }

        }

        void UpdateSuspitionValue(bool increaseSuspition)
        {
            float previousSuspition = this.currentSuspition;
            float speed = 100.0f / (increaseSuspition ? this.suspitionIncreaseTime : this.suspitionDecreaseTime);
            if (increaseSuspition)
            {
                this.currentSuspition += speed * Time.deltaTime;
                OnSuspitionChange?.Invoke(true);
            }
            else
            {
                this.currentSuspition -= speed * Time.deltaTime;
                 OnSuspitionChange?.Invoke(false);
            }
            this.currentSuspition = Mathf.Clamp(this.currentSuspition, 0.0f, 100.0f);
        }

        //update enemy vision and lens with a color
        public void UpdateVisionColor(Color? color)
        {
            Material lensMaterial = this.enemyRenderer.materials[1];

            if (color.HasValue)
            {
                this.enemyVision.VisionLight.color = color.Value;
                //enable lens emission
                lensMaterial.EnableKeyword("_EMISSION");
                //we assume that the lens material is at index 1
                lensMaterial.SetColor("_EmissionColor", color.Value);
                this.enemyVision.VisionLight.enabled = true;
            }
            else
            {
                lensMaterial.DisableKeyword("_EMISSION");
                this.enemyVision.VisionLight.enabled = false;
            }
        }

        private void OnDrawGizmosSelected()
        {
            
            for(int i = 0;i < this.patrolData.TotalPointsCount();i++)
            {
                PatrolPoint patrolPoint = this.patrolData.GetPoint(i);
                Gizmos.color = Color.white;
                Gizmos.DrawSphere(patrolPoint.position, 0.4f);

                Gizmos.color = Color.red;
                if (patrolPoint.rotationType == PatrolPoint.RotationType.FIXED)
                    Gizmos.DrawLine(patrolPoint.position, patrolPoint.position + Quaternion.AngleAxis(patrolPoint.fixedAngle, Vector3.up) * Vector3.forward);
            }
        }
        


        public void OnTriggerEnter(Collider collider)
        {
            if (collider.CompareTag("Player") && stateMachine.CurrentState.CanAttackPlayer())
            {
                this.stateMachine.SetState(new QteStateEnemy(this));
            }
        }

        public void ResetSuspition()
        {
            this.currentSuspition = 0;
            OnSuspitionReset?.Invoke();
        }

        //level of suspition from 0 to 1
        public float SuspitionLevel
        {
            get => this.currentSuspition / 100.0f;
        }
        
        public Transform TargetTransform
        {
            get => this.visionTarget.transform;
        }
    }
}