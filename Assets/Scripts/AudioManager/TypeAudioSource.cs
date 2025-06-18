using UnityEngine;
using UnityEngine.Rendering;


[RequireComponent(typeof(AudioSource))]
public class TypeAudioSource : MonoBehaviour
{

    private AudioSource audioSource;

    [Tooltip("The type of the audio")]
    public AudioType audioType;


    [SerializeField]
    private float baseAudioVolume = 1.0f;
    private float volumeScale = 1.0f;


    public float Volume
    {
        get => this.baseAudioVolume;
        set
        {
            this.baseAudioVolume = value;
            UpdateAudioSourceVolume();
        }
    }

    public AudioType AudioType
    {
        get => this.audioType;
        set
        {
            this.audioType = value;
            this.volumeScale = Managers.audioManager.GetVolume(this.audioType);
            UpdateAudioSourceVolume();
        }
    }

    void Awake()
    {
        this.audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        this.volumeScale = Managers.audioManager.GetVolume(this.audioType);
        Volume = this.baseAudioVolume;
        AudioManager.OnVolumeChange += OnVolumeChange;
    }

    void OnDestroy()
    {
        AudioManager.OnVolumeChange -= OnVolumeChange;
    }

    void UpdateAudioSourceVolume()
    {
        this.audioSource.volume = this.baseAudioVolume * this.volumeScale;
    }

    void OnVolumeChange(AudioType audioType, float volumeScale)
    {
        if (audioType == this.audioType)
        {
            this.volumeScale = volumeScale;
            UpdateAudioSourceVolume();
        }
    }

    public AudioSource Source
    {
        get => this.audioSource;
    }

}