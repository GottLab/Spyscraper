using System.Collections;
using UnityEngine;

public class Credits : MonoBehaviour
{
    [SerializeField, Tooltip("Delay before starting credits")]
    private float delay;

    [SerializeField]
    private Animator creditsAnimator;

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
        
    }


    private IEnumerator StartCreditsDelay()
    {
        yield return new WaitForSeconds(delay);
        this.creditsAnimator.gameObject.SetActive(true);
    }

    bool AreCreditsFinished()
    {
        return this.creditsAnimator.isActiveAndEnabled && this.creditsAnimator.GetAnimatorTransitionInfo(0).normalizedTime >= 1.0f;
    }
}
