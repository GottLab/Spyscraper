using System.Collections;
using MyGameDevTools.SceneLoading;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(LoadingBehavior))]
public class LoadingScreen : MonoBehaviour
{
    LoadingBehavior _loadingBehavior;
    LoadingProgress _loadingProgress;

    [SerializeField]
    private Image panelImage;


    [SerializeField]
    private AnimationCurve animationCurve = AnimationCurve.Linear(0.0f, 0.0f, 1.0f, 1.0f);

    [SerializeField]
    private float panelAnimationTime = 0.5f;

    void Awake()
    {
        _loadingBehavior = GetComponent<LoadingBehavior>();

        _loadingProgress = _loadingBehavior.Progress;

        StartCoroutine(StartTransition());


    }

    IEnumerator StartTransition()
    {
        yield return StartCoroutine(AnimateBlackCover(1.0f, 0.0f));
        this._loadingProgress.StartTransition();

        yield return StartCoroutine(AnimateBlackCover(0.0f, -1.0f));
        this._loadingProgress.EndTransition();
    }

    private IEnumerator AnimateBlackCover(float fromHeight, float toHeight)
    {
        float elapsed = 0f;

        Vector2 offsetMin = panelImage.rectTransform.offsetMin;
        Vector2 offsetMax = panelImage.rectTransform.offsetMax;


        RectTransform canvaRect = panelImage.canvas.GetComponent<RectTransform>();


        while (elapsed < panelAnimationTime)
        {
            float t = animationCurve.Evaluate(elapsed / panelAnimationTime);
            float value = Mathf.Lerp(fromHeight, toHeight, t);

            panelImage.rectTransform.offsetMin = new Vector2(offsetMin.x, value * canvaRect.rect.size.y);
            panelImage.rectTransform.offsetMax = new Vector2(offsetMax.x, value * canvaRect.rect.size.y);

            elapsed += Time.unscaledDeltaTime;
            yield return null;
        }

        panelImage.rectTransform.offsetMin = new Vector2(offsetMin.x, -canvaRect.rect.size.y);
        panelImage.rectTransform.offsetMax = new Vector2(offsetMax.x, -canvaRect.rect.size.y);

    }

}
