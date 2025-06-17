

using System.Collections;
using Enemy;
using UnityEngine;

public class InspectStateEnemy : IBehaviourState
{

    private readonly StateEnemyAI stateAI;
    private readonly Transform suspitiousTarget;

    private Coroutine inspectingCoroutine;

    public InspectStateEnemy(StateEnemyAI stateEnemyAI, Transform suspitiousTarget)
    {
        this.stateAI = stateEnemyAI;
        this.suspitiousTarget = suspitiousTarget;
    }

    public void Start()
    {
        this.stateAI.NavMeshAgent.destination = suspitiousTarget.position;
        this.stateAI.OnSuspitionChange += OnSuspitionChange;
    }

    IEnumerator InspectAround()
    {
        yield return new WaitForSeconds(3.0f);
        this.stateAI.StateMachine.SetState(new PatrolStateEnemy(this.stateAI));
    }

    public void Update()
    {
        if (this.stateAI.SuspitionLevel >= 1.0f)
            this.stateAI.StateMachine.SetState(new AttackStateEnemy(this.stateAI, this.stateAI.TargetTransform));

        if (this.inspectingCoroutine == null && !stateAI.NavMeshAgent.pathPending && stateAI.NavMeshAgent.remainingDistance < 0.5f)
        {
            this.inspectingCoroutine = this.stateAI.StartCoroutine(InspectAround());
        }

    }

    void OnSuspitionChange(bool increase)
    {
        if (increase)
            this.stateAI.NavMeshAgent.destination = this.stateAI.TargetTransform.position;
    }

    public void End()
    {
        if (this.inspectingCoroutine != null)
            this.stateAI.StopCoroutine(this.inspectingCoroutine);
        this.stateAI.NavMeshAgent.ResetPath();
        this.stateAI.OnSuspitionChange -= OnSuspitionChange;
    }
}