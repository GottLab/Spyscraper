
using UnityEngine;

public class AudioSetter : MonoBehaviour
{
    [SerializeField]
    private AudioType audioType;


    public void UpdateAudio(float volume)
    {
        Managers.audioManager.SetVolume(audioType, volume);
    }
}