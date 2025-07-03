using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Splines;
using Random = UnityEngine.Random;

namespace Enemy
{

    public enum PatrolType
    {
        Circle,
        Sequential,
        Random
    }

    [Serializable]
    public struct PatrolPoint
    {
        public enum RotationType
        {
            NONE,
            FIXED,
            AROUND
        }

        public Vector3 position;

        public RotationType rotationType;

        [Tooltip("Angle in degrees to turn until it does a 360 rotation, if 0 then the enemy won't turn at tall"), Range(0.0f, 360f)]
        public float turnAngle;

        [Tooltip("the time the enemy waits before turning, if turnAngle is 0 this value is ignored"), Min(0.0f)]
        public float turnWaitTime;

        [Tooltip("The angle it will look after reaching this point"), Min(0.0f)]
        public float fixedAngle;
    }

    [Serializable]
    public struct PatrolData
    {
        public PatrolPoint[] patrolPoints;
        public PatrolType patrolType;

        [Tooltip("Spline that the enemy will follow")]
        public SplineContainer spline;

        [Tooltip("Steps that divide the spline")]
        public int splineSteps;

        public PatrolPoint GetPoint(int index)
        {
            if (spline != null)
            {
                PatrolPoint patrolPoint = new PatrolPoint();
                patrolPoint.rotationType = PatrolPoint.RotationType.NONE;
                patrolPoint.position = spline.EvaluatePosition(index / (float)this.splineSteps);
                return patrolPoint;
            }
            else
            {
                return this.patrolPoints[index];
            }
        }

        public int TotalPointsCount()
        {
            return spline != null ? splineSteps : patrolPoints.Length;
        }

        
    }

    public class PatrolStateEnemy : IBehaviourState
    {

        private readonly StateEnemyAI stateAI;
        private int destPoint;
        private readonly PatrolData patrolData;
        private bool isMovingForward = true;

        //coroutine that handles the rotation behaviour
        private Coroutine turningCoroutine;

        //index used to tell the enemy to turn left (-1) or right (1) or stay still (0)
        private int turnDirection = 0;
        //accelleration to reach turn speed
        private float turnSpeed = 0.1f;

        public PatrolStateEnemy(StateEnemyAI stateEnemyAI)
        {
            stateAI = stateEnemyAI;
            patrolData = stateEnemyAI.PatrolData;
            stateEnemyAI.NavMeshAgent.ResetPath();
        }

        public void Start()
        {
            this.destPoint = FindNearestPointIndex();
            GotoNextPoint();
        }

        private void GotoNextPoint()
        {
            PatrolPoint patrolPoint = this.patrolData.GetPoint(this.destPoint);
            // Set the agent to go to the currently selected destination.
            stateAI.NavMeshAgent.destination = patrolPoint.position;

            int totalPoints = this.patrolData.TotalPointsCount();

            // Choose the next point in the array as the destination,
            // cycling to the start if necessary.
            switch (patrolData.patrolType)
            {
                case PatrolType.Circle:
                    destPoint = (destPoint + 1) % totalPoints;
                    break;

                case PatrolType.Sequential:
                    var forward = isMovingForward ? 1 : -1;
                    destPoint += forward;
                    if (destPoint >= totalPoints || destPoint < 0)
                    {
                        isMovingForward = !isMovingForward;
                        destPoint -= forward * 2;
                    }
                    break;

                case PatrolType.Random:
                    destPoint = Random.Range(0, totalPoints);
                    break;
            }
        }

        private IEnumerator RotateAndGotoNextPoint()
        {
            PatrolPoint currentPoint = this.patrolData.GetPoint(this.destPoint);
            switch (currentPoint.rotationType)
            {
                case PatrolPoint.RotationType.AROUND:
                    //Stop agent path
                    stateAI.NavMeshAgent.ResetPath();
                    //the amount of turns required to do a 360 degrees rotation
                    int amountOfTurns = (int)(360f / currentPoint.turnAngle);

                    float startAngle = this.stateAI.transform.eulerAngles.y;

                    for (int i = 0; i < amountOfTurns; i++)
                    {
                        float targetAngle = startAngle + i * currentPoint.turnAngle;
                        yield return RotateToAngle(targetAngle);
                        yield return new WaitForSeconds(currentPoint.turnWaitTime);
                    }
                    break;
                case PatrolPoint.RotationType.FIXED:
                    yield return RotateToAngle(currentPoint.fixedAngle);
                    break;
            }
            GotoNextPoint();
            turningCoroutine = null;
        }



        private IEnumerator RotateToAngle(float targetAngle)
        {
            float direction = Mathf.DeltaAngle(targetAngle, this.stateAI.transform.eulerAngles.y);

            //do not rotate if target is less that 5 degrees from current rotation
            if (Math.Abs(direction) <= 5.0f)
                yield break;

            turnDirection = -Math.Sign(direction);

            while (-Math.Sign(Mathf.DeltaAngle(targetAngle, this.stateAI.transform.eulerAngles.y)) == turnDirection)
            {
                yield return null;
            }
            turnDirection = 0;
            while (Math.Abs(Mathf.DeltaAngle(targetAngle, this.stateAI.transform.eulerAngles.y)) >= 1.0f)
            {
                this.stateAI.transform.rotation = Quaternion.Slerp(this.stateAI.transform.rotation, Quaternion.AngleAxis(targetAngle, Vector3.up), Time.deltaTime);
                yield return null;
            }

        }

        //find the index of the nearest patrol point
        int FindNearestPointIndex()
        {
            if (this.patrolData.TotalPointsCount() <= 0)
                return -1;

            Vector3 enemyPosition = this.stateAI.transform.position;
            float minDistanceSqr = Mathf.Infinity;
            int closestIndex = -1;

            for (int i = 0; i < this.patrolData.TotalPointsCount(); i++)
            {
                float distSqr = (this.patrolData.GetPoint(i).position - enemyPosition).sqrMagnitude;
                if (distSqr < minDistanceSqr)
                {
                    minDistanceSqr = distSqr;
                    closestIndex = i;
                }
            }
            return closestIndex;
        }


        public void Update()
        {
            // Choose the next destination point when the agent gets
            // close to the current one.
            if (turningCoroutine == null && !stateAI.NavMeshAgent.pathPending && stateAI.NavMeshAgent.remainingDistance < 0.5f)
                turningCoroutine = this.stateAI.StartCoroutine(RotateAndGotoNextPoint());

            this.stateAI.enemyAnimation.SetTurn(this.turnDirection, this.turnSpeed);

            if (this.stateAI.SuspitionLevel >= 1.0f)
                this.stateAI.StateMachine.SetState(new AttackStateEnemy(this.stateAI, this.stateAI.TargetTransform));
            else if (this.stateAI.SuspitionLevel > 0.0f)
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