using System;
using UnityEngine;
using UnityEngine.AI;

namespace Enemy
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class StateEnemyAI : MonoBehaviour
    {
        private BehaviourStateMachine stateMachine;
        [SerializeField] 
        private Vector3[] patrolTargets;
        [SerializeField]
        private PatrolStateEnemy.PatrolType patrolType;
        public Vector3[] PatrolTargets => patrolTargets;
        private NavMeshAgent navMeshAgent;
        
        public NavMeshAgent NavMeshAgent => navMeshAgent;
        
        void Start()
        {
            stateMachine = new BehaviourStateMachine();
            navMeshAgent = GetComponent<NavMeshAgent>();
            stateMachine.SetState(new PatrolStateEnemy(this, patrolType));
        }

        void Update()
        {
            stateMachine.Update();
        }

        private void OnDrawGizmos()
        {
            foreach (var patrolTarget in patrolTargets)
            {
                Gizmos.DrawSphere(patrolTarget, 1f);
            }
        }
    }
}