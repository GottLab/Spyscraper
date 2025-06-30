using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class DeathScreen : MonoBehaviour
{

    [SerializeField]
    private Image deathMessage;

    [SerializeField]
    private CanvasGroup deathCanvas;


    [SerializeField]
    private float fadeInTime = 1.0f;

    [SerializeField]
    private Sprite[] deathMessages;

    [SerializeField]
    private Ease animationEase;


    public void StartDeathScreen()
    {

        Sprite deathSprite = deathMessages[Random.Range(0, deathMessages.Length)];

        deathMessage.sprite = deathSprite;

        float deathProgress = 0.0f;
        DOTween.To(() => deathProgress,
            x =>
            {
                this.deathCanvas.alpha = x;
            }, 1.0f, fadeInTime).SetEase(animationEase);
    }

    


}
