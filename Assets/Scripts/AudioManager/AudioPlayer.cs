using UnityEngine;

public class AudioPlayer : MonoBehaviour
{
    [SerializeField]
    private AudioType audioType;

    [SerializeField]
    private float volume = 1.0f;

    [SerializeField]
    private float pitch = 1.0f;

    [SerializeField]
    private Transform sourceTransform;
    
    [SerializeField]
    private AudioClip[] audioClips;


    public void PlayAudio()
    {
        AudioClip clip = this.audioClips[Random.Range(0, audioClips.Length)];
        Vector3? position = this.sourceTransform == null ? null : this.sourceTransform.position;
        Managers.audioManager.PlayClipAtPoint(clip, position, volume: this.volume, pitch: this.pitch, audioType: this.audioType);
    }

}