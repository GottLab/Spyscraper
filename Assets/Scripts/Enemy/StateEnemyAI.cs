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

        [SerializeField]
        private QteSequence qteSequence;
        public QteSequence QteSequence => qteSequence;

        [SerializeField]
        private PatrolStateEnemy.PatrolData patrolData;
        public PatrolStateEnemy.PatrolData PatrolData => patrolData;

        private NavMeshAgent navMeshAgent;
        public NavMeshAgent NavMeshAgent => navMeshAgent;

        [NonSerialized]
        public EnemyAnimation enemyAnimation;

        [SerializeField, Tooltip("Cone vision used by the enemy")]
        private ConeVision enemyVision;

        [SerializeField, Tooltip("Target of enemy vision that raises its suspition")]
        private ConeVisionObject visionTarget;


        //SUSPITION VARIABLES

        //suspition level of the enemy it goes from 0 to 100
        private float currentSuspition = 0.0f;

        //the last time the enemy increased it's suspition level
        private double lastTimeIncrease;

        [SerializeField, Tooltip("Time it takes in seconds to fill up suspition")]
        private float suspitionIncreaseTime = 1.0f;

        [SerializeField, Tooltip("Time it takes in seconds to empty suspition")]
        private float suspitionDecreaseTime = 1.0f;

        [SerializeField, Tooltip("How much time in seconds needs to pass before decreasing suspition")]
        private float suspitionAvoidanceTime = 1.0f;

        //called when suspition level increases or decreases
        public event Action<float> OnSuspitionChange;


        void Start()
        {
            stateMachine = new BehaviourStateMachine();
            navMeshAgent = GetComponent<NavMeshAgent>();
            stateMachine.SetState(new PatrolStateEnemy(this));
            enemyAnimation = GetComponent<EnemyAnimation>();
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
            }
            else
            {
                this.currentSuspition -= speed * Time.deltaTime;
            }
            this.currentSuspition = Mathf.Clamp(this.currentSuspition, 0.0f, 100.0f);
            if (previousSuspition != this.currentSuspition)
            {
                OnSuspitionChange?.Invoke(this.currentSuspition);
            }
        }



        private void OnDrawGizmos()
        {
            foreach (var patrolTarget in patrolData.patrolTargets)
            {
                Gizmos.DrawSphere(patrolTarget, 1f);
            }
        }


        public void OnTriggerEnter(Collider collider)
        {
            if (collider.CompareTag("Player") && stateMachine.CurrentState.CanAttackPlayer())
            {
                this.stateMachine.SetState(new QteStateEnemy(this));
            }
        }

        //level of suspition from 0 to 1
        public float SuspitionLevel
        {
            get => this.currentSuspition / 100.0f;
        }
    }
}