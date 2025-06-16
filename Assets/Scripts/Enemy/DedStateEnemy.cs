


using Enemy;
using UnityEngine;

public class DedStateEnemy : IBehaviourState
{
    private readonly StateEnemyAI stateAI;
    private Collider[] colliders;

    public DedStateEnemy(StateEnemyAI stateEnemyAI)
    {
        stateAI = stateEnemyAI;
    }

    public void Start()
    {
        stateAI.enemyAnimation.StartDeath();
        //random rotation on y axis on death
        stateAI.transform.rotation = Quaternion.Euler(0, Random.Range(0f, 360f), 0);


        colliders = stateAI.GetComponents<Collider>();
        SetCollider(false);
    }

    public void End()
    {
        SetCollider(true);
    }

    public void Update()
    {
    }

    private void SetCollider(bool enabled)
    {
        foreach(Collider collider in colliders)
        {
            collider.enabled = enabled;
        }
    }
    
    public bool CanAttackPlayer()
    {
        return false;
    }
}