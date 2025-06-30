using System.Collections;
using Enemy;
using UnityEngine;

[RequireComponent(typeof(Animator)), RequireComponent(typeof(StateEnemyAI))]
public class EnemyAnimation : MonoBehaviour
{

    private Animator animator;
    private StateEnemyAI stateEnemyAI;
    private Light agentLight;

    //ANIMATOR PROPERTIES    
    private static readonly int SpeedProperty = Animator.StringToHash("Speed");
    private static readonly int AttackProperty = Animator.StringToHash("Attack");
    private static readonly int DeathProperty =  Animator.StringToHash("Death");
    private static readonly int HurtProperty = Animator.StringToHash("Hurt");
    private static readonly int TurnProperty = Animator.StringToHash("Turn");
    private bool hitPunch = false;

    [SerializeField]
    private AudioPlayer punchHit;

    [SerializeField]
    private AudioPlayer punchReceive;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
        stateEnemyAI = GetComponent<StateEnemyAI>();
        agentLight = GetComponentInChildren<Light>();
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
        punchHit?.PlayAudio();
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
        this.punchReceive?.PlayAudio();
    }

    public void SetTurn(int turn, float speed = 0.1f)
    {
        if (speed == -1.0f)
        {
            this.animator.SetFloat(TurnProperty, turn);
        }
        else
        {
            this.animator.SetFloat(TurnProperty, turn, speed, Time.deltaTime);
        }
    }
}
