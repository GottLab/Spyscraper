using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class AudioSlider : MonoBehaviour, IEndDragHandler
{
    [SerializeField]
    private AudioType audioType;

    private Slider slider;

    public void Start()
    {
        this.slider = GetComponent<Slider>();

        StartCoroutine(Managers.WaitForManagerStatus(Managers.audioManager, ManagerStatus.Started, () =>
        {
            this.slider.value = Managers.audioManager.GetVolume(this.audioType);
        }));

        this.slider.onValueChanged.AddListener((value) =>
        {
            UpdateAudio(false);
        });
    }

    public void UpdateAudio(bool saveToFile)
    {
        Managers.audioManager.SetVolume(audioType, this.slider.value, saveToFile);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        UpdateAudio(true);
    }
}