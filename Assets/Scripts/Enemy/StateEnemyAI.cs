using System;
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
        private PatrolStateEnemy.PatrolData patrolData;
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

        void OnCollisionEnter(Collision collision)
        {
            this.stateMachine.CurrentState.OnCollide(collision);
            if (collision.gameObject.CompareTag("Player"))
            {
                this.stateMachine.SetState(new QteStateEnemy(this));
            }
        }
    }
}