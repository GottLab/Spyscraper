using DG.Tweening;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
public class QtePostProcessEffect : MonoBehaviour
{
    public Material material;

    [SerializeField, Tooltip("Time it takes in seconds to fade in this effect")]
    private float fadeInTime = 1.0f;

    private Tween fadeTween;

    private Material qteMaterial;

    void OnEnable()
    {
        qteMaterial = new Material(material);
        qteMaterial.hideFlags = HideFlags.HideAndDontSave;
        OnQteEnd(true);
        QTEManager.OnQteElementStart += OnQteStart;
        QTEManager.OnQteElementEnd += OnQteEnd;
    }

    void OnDisable()
    {
        DestroyImmediate(qteMaterial);
        qteMaterial = null;
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
                    qteMaterial.SetFloat("_Amount", intensity);
                },
                1.0f, fadeInTime).SetUpdate(true);
    }

    void OnQteEnd(bool success)
    {
        fadeTween?.Kill();
        qteMaterial.SetFloat("_Amount", 0.0f);
    }


    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (qteMaterial != null && qteMaterial.GetFloat("_Amount") > 0.0f)
            Graphics.Blit(source, destination, qteMaterial);
        else
            Graphics.Blit(source, destination);
    }
}
