using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Enemy
{
    public class PatrolStateEnemy : IBehaviourState
    {
        public enum PatrolType
        {
            Circle,
            Sequential,
            Random
        }
        
        [Serializable]
        public struct PatrolData
        {
            public Vector3[] patrolTargets;
            public PatrolStateEnemy.PatrolType patrolType;
        }

        private readonly StateEnemyAI stateAI;
        private int destPoint;
        private readonly PatrolData patrolData;
        private bool isMovingForward = true;
        
        public PatrolStateEnemy(StateEnemyAI stateEnemyAI)
        {
            stateAI = stateEnemyAI;
            patrolData = stateEnemyAI.PatrolData;
        }
        
        public void Start()
        {
            stateAI.NavMeshAgent.autoBraking = false;
            GotoNextPoint();
        }
        
        private void GotoNextPoint() {
            // Returns if no points have been set up
            if (patrolData.patrolTargets.Length == 0)
                return;

            // Set the agent to go to the currently selected destination.
            stateAI.NavMeshAgent.destination = patrolData.patrolTargets[destPoint];

            // Choose the next point in the array as the destination,
            // cycling to the start if necessary.

            switch (patrolData.patrolType)
            {
                case PatrolType.Circle:
                    destPoint = (destPoint + 1) % patrolData.patrolTargets.Length;
                    break;
                
                case PatrolType.Sequential:
                    var forward = isMovingForward ? 1 : -1; 
                    destPoint = (destPoint + forward);
                    if (destPoint >= patrolData.patrolTargets.Length || destPoint < 0)
                    {
                        isMovingForward = !isMovingForward;
                        destPoint -= forward * 2;
                    }
                    break;
                
                case PatrolType.Random:
                    destPoint = Random.Range(0, patrolData.patrolTargets.Length);
                    break;
            }
        }


        public void Update () {
            // Choose the next destination point when the agent gets
            // close to the current one.
            if (!stateAI.NavMeshAgent.pathPending && stateAI.NavMeshAgent.remainingDistance < 0.5f)
                GotoNextPoint();
        }

        public void End()
        {
            stateAI.NavMeshAgent.ResetPath();
        }
    }
}