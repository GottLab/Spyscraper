using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonScaler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
{
    private Vector3 originalScale;
    public Vector3 targetScale = new Vector3(1.1f, 1.1f, 1.1f);
    public float scaleSpeed = 5f;


    private int scaleCounter = 0;

    void Start()
    {
        originalScale = transform.localScale;
    }

    void Update()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, this.scaleCounter > 0 ? targetScale : originalScale, Time.unscaledDeltaTime * scaleSpeed);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        scaleCounter += 1;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        scaleCounter -= 1;
    }

    public void OnSelect(BaseEventData eventData)
    {
        scaleCounter += 1;
    }

    public void OnDeselect(BaseEventData eventData)
    {
        scaleCounter -= 1;
    }
}