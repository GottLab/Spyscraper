using System.Collections;
using UnityEngine;

public class Credits : MonoBehaviour
{
    [SerializeField, Tooltip("Delay before starting credits")]
    private float delay;

    [SerializeField]
    private Animator creditsAnimator;

    private bool started;

    private readonly int SpeedProperty = Animator.StringToHash("Speed");

    public void StartCredits()
    {
        StartCoroutine(StartCreditsDelay());
    }

    void Update()
    {
        if (AreCreditsFinished() && GameManager.GetKeyDown(KeyCode.Return))
        {
            Managers.game.TransitionScene("MainMenu");
        }

        if (started)
        {
            this.creditsAnimator.SetFloat(SpeedProperty, GameManager.GetKey(KeyCode.Return) ? 5.0f : 1.0f);
        }
        
    }


    private IEnumerator StartCreditsDelay()
    {
        yield return new WaitForSeconds(delay);
        started = true;
        this.creditsAnimator.gameObject.SetActive(true);
        Managers.audioManager.PlayMusic(Music.Credits, 2.0f);
    }

    bool AreCreditsFinished()
    {
        return started && this.creditsAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f;
    }
}
