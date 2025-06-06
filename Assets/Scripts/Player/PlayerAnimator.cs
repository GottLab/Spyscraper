using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerAnimator : MonoBehaviour
{

    private Animator animator;
    private readonly int StateProperty = Animator.StringToHash("State");
    private readonly int ChangeStateProperty = Animator.StringToHash("ChangeState");
    private readonly int LeftPunchProperty = Animator.StringToHash("LeftPunch");
    private readonly int RightPunchProperty = Animator.StringToHash("RightPunch");

    private bool didPunchHit = false;

    void Start()
    {
        this.animator = GetComponent<Animator>();

        Managers.playerManager.OnStatusChange += OnPlayerStateChange;

    }

    void OnDestroy()
    {
        Managers.playerManager.OnStatusChange -= OnPlayerStateChange;
    }

    // Update Current Player Animation Based on his State
    void OnPlayerStateChange(PlayerManager.PlayerState playerState)
    {
        //set the state index
        animator.SetInteger(StateProperty, (int)playerState);
        //trigger the transition to it
        animator.SetTrigger(ChangeStateProperty);
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

        yield return new WaitUntil(() => this.didPunchHit || currentTime > 3.0f);
    }

    //this is called by the player punch event, this is why this method is public
    public void OnPunchHit()
    {
        didPunchHit = true;
    }
}
