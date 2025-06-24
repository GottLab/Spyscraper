using System;
using System.Collections;
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

            [Tooltip("Angle in degrees to turn until it does a 360 rotation, if 0 then the enemy won't turn at tall"), Range(0.0f,360f)]
            public float turnAngle;
            
            [Tooltip("the time the enemy waits before turning, if turnAngle is 0 this value is ignored"), Min(0.0f)]
            public float turnWaitTime;
        }

        private readonly StateEnemyAI stateAI;
        private int destPoint;
        private readonly PatrolData patrolData;
        private bool isMovingForward = true;

        //coroutine that handles the rotation behaviour
        private Coroutine turningCoroutine;

        //index used to tell the enemy to turn left (-1) or right (1) or stay still (0)
        private int turnDirection = 0;

        public PatrolStateEnemy(StateEnemyAI stateEnemyAI)
        {
            stateAI = stateEnemyAI;
            patrolData = stateEnemyAI.PatrolData;
            stateEnemyAI.NavMeshAgent.ResetPath();
        }
        
        public void Start()
        {
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
                    destPoint += forward;
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

        private IEnumerator RotateAndGotoNextPoint()
        {
            if (this.patrolData.turnAngle > 0.0f)
            {
                //Stop agent path
                stateAI.NavMeshAgent.ResetPath();
                //the amount of turns required to do a 360 degrees rotation
                int amountOfTurns = (int)(360f / this.patrolData.turnAngle);

                float startAngle = this.stateAI.transform.eulerAngles.y;

                for (int i = 0; i < amountOfTurns; i++)
                {
                    float targetAngle = startAngle + (1 + i) * this.patrolData.turnAngle;
                    turnDirection = 1;
                    while (Mathf.DeltaAngle(targetAngle, this.stateAI.transform.eulerAngles.y) >= 0.1f)
                    {
                        yield return null;
                    }
                    turnDirection = 0;
                    yield return new WaitForSeconds(this.patrolData.turnWaitTime);
                }
            }
            GotoNextPoint();
            turningCoroutine = null;
        }


        public void Update()
        {
            // Choose the next destination point when the agent gets
            // close to the current one.
            if (turningCoroutine == null && !stateAI.NavMeshAgent.pathPending && stateAI.NavMeshAgent.remainingDistance < 0.5f)
                turningCoroutine = this.stateAI.StartCoroutine(RotateAndGotoNextPoint());

            this.stateAI.enemyAnimation.SetTurn(this.turnDirection);

            if (this.stateAI.SuspitionLevel >= 1.0f)
                this.stateAI.StateMachine.SetState(new AttackStateEnemy(this.stateAI, this.stateAI.TargetTransform));
            else if(this.stateAI.SuspitionLevel > 0.0f)
                this.stateAI.StateMachine.SetState(new InspectStateEnemy(this.stateAI, this.stateAI.TargetTransform));

        }

        public void End()
        {   
            if (this.turningCoroutine != null)
                this.stateAI.StopCoroutine(this.turningCoroutine);
            stateAI.NavMeshAgent.ResetPath();
            stateAI.NavMeshAgent.velocity = Vector3.zero;
        }
    }
}