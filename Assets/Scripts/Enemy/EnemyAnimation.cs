using System.Collections;
using Enemy;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Animator)), RequireComponent(typeof(StateEnemyAI))]
public class EnemyAnimation : MonoBehaviour
{

    private Animator animator;
    private StateEnemyAI stateEnemyAI;
    private Light agentLight;


    //ANIMATOR PROPERTIES    
    private int SpeedProperty;
    private int AttackProperty;
    private int DeathProperty;
    private int HurtProperty;

    private int TurnProperty;


    private bool hitPunch = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
        stateEnemyAI = GetComponent<StateEnemyAI>();
        agentLight = GetComponentInChildren<Light>();
        SpeedProperty = Animator.StringToHash("Speed");
        AttackProperty = Animator.StringToHash("Attack");
        DeathProperty = Animator.StringToHash("Death");
        HurtProperty = Animator.StringToHash("Hurt");
        TurnProperty = Animator.StringToHash("Turn");

        this.stateEnemyAI.UpdateVisionColor(Color.red);
    }

    // Update is called once per frame
    void Update()
    {
        this.animator.SetFloat(SpeedProperty, this.stateEnemyAI.NavMeshAgent.velocity.magnitude, 0.1f, Time.deltaTime);
    }

    public IEnumerator Attack()
    {
        hitPunch = false;
        this.animator.SetTrigger(AttackProperty);
        yield return new WaitUntil(() => hitPunch);
        hitPunch = false;
    }

    //this is called by the enemy punch event, this is why this method is public
    public void OnPunchHit()
    {
        hitPunch = true;
    }

    public void StartDeath()
    {
        this.agentLight.enabled = false;
        this.animator.ResetTrigger(HurtProperty);
        this.animator.SetTrigger(DeathProperty);
        this.stateEnemyAI.UpdateVisionColor(null);
    }

    public void StartHit()
    {
        this.animator.SetTrigger(HurtProperty);
    }

    public void SetTurn(int turn)
    {
        this.animator.SetFloat(TurnProperty, turn, 0.1f, Time.deltaTime);
    }
}
