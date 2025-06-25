using System.Collections;
using QTESystem;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerAnimator : MonoBehaviour, IQtePlayer
{

    private Animator animator;
    private static readonly int StateProperty = Animator.StringToHash("State");
    private static readonly int ChangeStateProperty = Animator.StringToHash("ChangeState");
    private static readonly int LeftPunchProperty = Animator.StringToHash("LeftPunch");
    private static readonly int RightPunchProperty = Animator.StringToHash("RightPunch");

    private bool didPunchHit = false;

    [SerializeField]
    private AudioPlayer punchSound;

    void Start()
    {
        this.animator = GetComponent<Animator>();
    }

    void OnEnable()
    {
        PlayerManager.OnStatusChange += OnPlayerStateChange;
    }

    void OnDisable()
    {
        PlayerManager.OnStatusChange -= OnPlayerStateChange;
    }

    // Update Current Player Animation Based on his State
    void OnPlayerStateChange(PlayerManager.PlayerState playerState)
    {   
        if (animator)
        {
            //set the state index
            animator.SetInteger(StateProperty, (int)playerState);
            //trigger the transition to it
            animator.SetTrigger(ChangeStateProperty);
        }
    }

    //makes the player throw a punch and wait until it lands
    public IEnumerator QteAttack()
    {
        
        didPunchHit = false;
        animator.ResetTrigger(LeftPunchProperty);
        animator.ResetTrigger(RightPunchProperty);

        bool LeftPunch = Random.Range(0, 2) == 0;
        animator.SetTrigger(LeftPunch ? LeftPunchProperty : RightPunchProperty);

        float currentTime = Time.time;

        yield return new WaitUntil(() => this.didPunchHit);
    }

    //this is called by the player punch event, this is why this method is public
    public void OnPunchHit()
    {
        didPunchHit = true;
        punchSound?.PlayAudio();
    }

    public void QteStart(IQtePlayer enemy)
    {
        Managers.playerManager.SetStatus(PlayerManager.PlayerState.QTE);


        Vector3 lookDirection = enemy.GetTransform().position - this.transform.position;
        lookDirection.y = 0.0f;
        this.transform.rotation = Quaternion.LookRotation(lookDirection);
    }

    public void QteSuccess()
    {
        Managers.playerManager.SetStatus(PlayerManager.PlayerState.NORMAL);
    }

    public void QteFail()
    {
        Managers.playerManager.SetStatus(PlayerManager.PlayerState.DIED);
    }

    public void QteOnHit()
    {
    }
}
