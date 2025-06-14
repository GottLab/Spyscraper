using UnityEngine;
using UnityEngine.AI;

namespace Enemy
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class StateEnemyAI : MonoBehaviour
    {
        private BehaviourStateMachine stateMachine;
        public BehaviourStateMachine StateMachine => stateMachine;
        [SerializeField]
        private QteSequence qteSequence;
        [SerializeField]
        private PatrolStateEnemy.PatrolData patrolData;
        public QteSequence QteSequence => qteSequence;
        public PatrolStateEnemy.PatrolData PatrolData => patrolData;
        private NavMeshAgent navMeshAgent;
        
        public NavMeshAgent NavMeshAgent => navMeshAgent;
        
        void Start()
        {
            stateMachine = new BehaviourStateMachine();
            navMeshAgent = GetComponent<NavMeshAgent>();
            stateMachine.SetState(new PatrolStateEnemy(this));
        }

        void Update()
        {
            stateMachine.Update();
        }

        private void OnDrawGizmos()
        {
            foreach (var patrolTarget in patrolData.patrolTargets)
            {   
                
                Gizmos.DrawSphere(patrolTarget, 1f);
            }
        }

        void OnTriggerEnter(Collider collider)
        {
            
        }

        public void OnPlayerCollide()
        {
            //this.stateMachine.CurrentState.OnCollide(collision);
            if (stateMachine.CurrentState.CanAttackPlayer())
            {
                this.stateMachine.SetState(new QteStateEnemy(this));
            }
        }
    }
}