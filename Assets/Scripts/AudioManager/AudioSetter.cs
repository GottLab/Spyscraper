
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class AudioSetter : MonoBehaviour
{
    [SerializeField]
    private AudioType audioType;

    private Slider slider;

    public void Start()
    {
        this.slider = GetComponent<Slider>();   
    }

    public void UpdateAudio(bool save)
    {
        Managers.audioManager.SetVolume(audioType, this.slider.value, save);
    }
}