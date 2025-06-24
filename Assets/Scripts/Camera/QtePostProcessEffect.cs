using DG.Tweening;
using QTESystem;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class QtePostProcessEffect : MonoBehaviour
{
    public Material material;

    [SerializeField, Tooltip("Time it takes in seconds to fade in this effect")]
    private float fadeInTime = 1.0f;


    private Tween fadeTween;


    void Awake()
    {
        material.SetFloat("_Amount", 0.0f);
    }


    void OnEnable()
    {
        QTEManager.OnQteElementStart += OnQteStart;
        QTEManager.OnQteElementEnd += OnQteEnd;
    }

    void OnDisable()
    {
        QTEManager.OnQteElementStart -= OnQteStart;
        QTEManager.OnQteElementEnd -= OnQteEnd;
    }

    void OnQteStart(KeyCode keyCode, float time)
    {
        float intensity = 0.0f;
        fadeTween = DOTween.To(() => intensity, 
                x =>
                {
                    intensity = x;
                    material.SetFloat("_Amount", intensity);
                },
                1.0f, fadeInTime).SetUpdate(true);
    }

    void OnQteEnd(bool success)
    {
        fadeTween.Kill();
        material.SetFloat("_Amount", 0.0f);
    }


    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (material != null && material.GetFloat("_Amount") > 0.0f)
            Graphics.Blit(source, destination, material);
        else
            Graphics.Blit(source, destination);
    }
}
