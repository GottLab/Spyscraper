
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class AudioSlider : MonoBehaviour
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
    }

    public void UpdateAudio(bool save)
    {
        Managers.audioManager.SetVolume(audioType, this.slider.value, save);
    }
}