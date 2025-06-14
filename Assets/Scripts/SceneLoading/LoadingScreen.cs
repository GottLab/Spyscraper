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

        yield return StartCoroutine(AnimateBlackCover(0.0f, 1.0f));
        this._loadingProgress.EndTransition();
    }

    private IEnumerator AnimateBlackCover(float fromHeight, float toHeight)
    {
        float elapsed = 0f;

        Vector2 deltaSize = panelImage.rectTransform.anchorMin;

        while (elapsed < panelAnimationTime)
        {
            float t = animationCurve.Evaluate(elapsed / panelAnimationTime);
            float value = Mathf.Lerp(fromHeight, toHeight, t);
            panelImage.rectTransform.anchorMin = new Vector2(deltaSize.x, value);

            elapsed += Time.deltaTime;
            yield return null;
        }

        panelImage.rectTransform.anchorMin = new Vector2(deltaSize.x, toHeight);

    }

}
