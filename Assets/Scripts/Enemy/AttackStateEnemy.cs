using UnityEngine;
using Enemy;
using System;

public class AttackStateEnemy : IBehaviourState
{

    [Serializable]
    public struct AttackData
    {
        [Tooltip("Color used when enemy attacks")]
        public Color color;

        [Tooltip("Speed at which enemy follows player")]
        public float speed;
    }

    private readonly StateEnemyAI stateEnemyAI;
    private readonly Transform target;


    public AttackStateEnemy(StateEnemyAI stateEnemyAI, Transform target)
    {
        this.stateEnemyAI = stateEnemyAI;
        this.target = target;
    }

    public void Start()
    {
        this.stateEnemyAI.UpdateVisionColor(this.stateEnemyAI.AttackData.color);
        this.stateEnemyAI.NavMeshAgent.speed = this.stateEnemyAI.AttackData.speed;
    }

    public void Update()
    {

        if (this.stateEnemyAI.SuspitionLevel == 0.0f)
        {
            this.stateEnemyAI.StateMachine.SetState(new PatrolStateEnemy(this.stateEnemyAI));
        }

        this.stateEnemyAI.NavMeshAgent.destination = this.target.position;
    }

    public void End()
    {
        this.stateEnemyAI.NavMeshAgent.speed = this.stateEnemyAI.defaultSpeed;
        this.stateEnemyAI.UpdateVisionColor(Color.yellow);
        this.stateEnemyAI.NavMeshAgent.ResetPath();
    }
}
