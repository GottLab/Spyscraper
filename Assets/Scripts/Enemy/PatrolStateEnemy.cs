using UnityEngine;

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

        private readonly StateEnemyAI stateAI;
        private int destPoint;
        private readonly PatrolType patrolType;
        private bool isMovingForward = true;
        
        public PatrolStateEnemy(StateEnemyAI stateEnemyAI, PatrolType type)
        {
            stateAI = stateEnemyAI;
            patrolType = type;
        }
        
        public void Start()
        {
            stateAI.NavMeshAgent.autoBraking = false;
            GotoNextPoint();
        }
        
        private void GotoNextPoint() {
            // Returns if no points have been set up
            if (stateAI.PatrolTargets.Length == 0)
                return;

            // Set the agent to go to the currently selected destination.
            stateAI.NavMeshAgent.destination = stateAI.PatrolTargets[destPoint];

            // Choose the next point in the array as the destination,
            // cycling to the start if necessary.

            switch (patrolType)
            {
                case PatrolType.Circle:
                    destPoint = (destPoint + 1) % stateAI.PatrolTargets.Length;
                    break;
                
                case PatrolType.Sequential:
                    var forward = isMovingForward ? 1 : -1; 
                    destPoint = (destPoint + forward);
                    if (destPoint >= stateAI.PatrolTargets.Length || destPoint < 0)
                    {
                        isMovingForward = !isMovingForward;
                        destPoint -= forward * 2;
                    }
                    break;
                
                case PatrolType.Random:
                    destPoint = Random.Range(0, stateAI.PatrolTargets.Length);
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