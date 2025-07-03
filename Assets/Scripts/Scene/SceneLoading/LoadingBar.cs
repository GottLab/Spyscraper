using MyGameDevTools.SceneLoading;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class LoadingBar : MonoBehaviour
{
    public LoadingBehavior loadingBehavior;

    Image loadingBarImage;

    void Awake()
    {
        loadingBarImage = GetComponent<Image>();
        loadingBarImage.fillAmount = 0;
    }

    void Start()
    {
        loadingBehavior.Progress.Progressed += UpdateLoadingBar;
    }

    private void UpdateLoadingBar(float progress) => loadingBarImage.fillAmount = progress;
}