using UnityEngine;

[ExecuteInEditMode]
public class MergeWithUI : MonoBehaviour
{
    public Camera otherCamera;

    public Shader shader;

    private Material blendMaterial;

    void OnEnable()
    {
        this.blendMaterial = new Material(shader);
    }

     void OnDisable()
    {
        DestroyImmediate(blendMaterial);
        blendMaterial = null;
    }


    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (otherCamera)
        {
            var texture = RenderTexture.GetTemporary(source.width, source.height, 0, source.format);
            otherCamera.targetTexture = texture;
            otherCamera.Render();

            blendMaterial.SetTexture("_OtherTex", texture);

            Graphics.Blit(source, destination, blendMaterial);

            RenderTexture.ReleaseTemporary(texture);
        }
        else
        {
            Graphics.Blit(source, destination);
        }
    }
}