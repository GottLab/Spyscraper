using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour, IGameManager
{
    private class Settings : ISaveData
    {
        public readonly Dictionary<AudioType, float> audioVolume = new();

        public Settings()
        {
            foreach (AudioType audioType in Enum.GetValues(typeof(AudioType)))
            {
                audioVolume[audioType] = 1.0f;
            }
        }

        public string Name()
        {
            return "audio_settings";
        }
    }


    private ManagerStatus currentStatus = ManagerStatus.Initializing;
    public ManagerStatus status => currentStatus;


    public static event Action<AudioType, Vector3?> OnSoundPlay;
    public static event Action<AudioType, float> OnVolumeChange;

    [SerializeField]
    private AudioMixer audioMixer;

    [SerializeField]
    private AudioMixerGroup[] mixerGroupTypes;

    [SerializeField, Tooltip("Audio Source used to play music")]
    private AudioSource musicAudioSource;

    //this is used to crossfade from one music to another
    private AudioSource transitionMusicSource;


    [SerializeField]
    private Music defaultMusic;

    private Settings audioSettings = new();

    private Queue<AudioSource> audioSources = new();
    
    //tells if we are using transitionMusicSource or musicAudioSource for music
    private bool switched = false;

    //are we transitioning from one musing to another
    private bool musicTransition;


    public void Startup()
    {

    }

    void Start()
    {
        SaveManager.saveManager.Load(this.audioSettings);

        foreach (AudioType audioType in Enum.GetValues(typeof(AudioType)))
        {
            SetVolume(audioType, this.audioSettings.audioVolume[audioType], save: false);
        }
        SaveSettings();

        this.transitionMusicSource = Instantiate(this.musicAudioSource);
        this.transitionMusicSource.transform.SetParent(this.musicAudioSource.transform);
        this.musicAudioSource.ignoreListenerPause = true;
        this.transitionMusicSource.ignoreListenerPause = true;
        this.PlayMusic(this.defaultMusic, 1.0f);

        this.currentStatus = ManagerStatus.Started;
    }

    void OnEnable()
    {
        GameManager.OnGamePause += OnGamePause;
    }

    void OnDisable()
    {
        GameManager.OnGamePause -= OnGamePause;
    }

    void OnGamePause(bool paused)
    {
        this.audioMixer.SetFloat("MusicFreqCutoff", paused ? 300.0f : 5000.0f);
        AudioListener.pause = paused;
    }

    void SaveSettings()
    {
        SaveManager.saveManager.Save(this.audioSettings);
    }

    public void SetVolume(AudioType audioType, float value, bool save = true)
    {

        float volumeLinear = Mathf.Clamp01(value);
        float volumeDb = Mathf.Log10(Mathf.Max(volumeLinear, 0.0001f)) * 20f;

        this.audioSettings.audioVolume[audioType] = value;
        this.audioMixer.SetFloat($"{audioType}Volume", volumeDb);
        OnVolumeChange?.Invoke(audioType, value);
        if (save)
        {
            SaveSettings();
        }
    }

    public float GetVolume(AudioType audioType)
    {
        return this.audioSettings.audioVolume[audioType];
    }


    public void PlayMusic(Music music, float transitionTime = 0.0f)
    {
        if (!musicTransition)
        {
            StartCoroutine(LoadAndPlayMusic(music, transitionTime));
        }
    }


    public void Play2DSound(AudioClip audioClip, float volume = 1.0f, AudioType audioType = AudioType.Master)
    {
        PlayClipAtPoint(audioClip, null, volume, audioType);
    }

    public void PlayClipAtPoint(AudioClip audioClip, Vector3? position = null, float volume = 1.0f, AudioType audioType = AudioType.Master)
    {
        if (audioClip == null)
        {
            Debug.LogWarning("Cannot play clip since it is null", this);
            return;
        }

        bool isSpatial = position.HasValue;
        AudioSource audioSource;
        if (this.audioSources.Count > 0)
        {
            audioSource = this.audioSources.Dequeue();
        }
        else
        {
            audioSource = new GameObject().AddComponent<AudioSource>();
            audioSource.loop = false;
            audioSource.gameObject.hideFlags = HideFlags.HideInHierarchy;
        }
        if (audioSource.IsDestroyed())
        {
            Debug.LogWarning("AudioSource Is destroyed", this);
            return;
        }
        audioSource.ignoreListenerPause = audioType == AudioType.Ui;
        audioSource.gameObject.SetActive(true);
        audioSource.outputAudioMixerGroup = GetAudioMixer(audioType);
        audioSource.spatialize = isSpatial;
        audioSource.spatialBlend = isSpatial ? 1.0f : 0.0f;
        audioSource.clip = audioClip;
        audioSource.volume = volume;
        audioSource.transform.position = isSpatial ? position.Value : Vector3.zero;

        OnSoundPlay?.Invoke(audioType, position);

        StartCoroutine(PlayAudioSource(audioSource));
    }


    IEnumerator PlayAudioSource(AudioSource audioSource)
    {
        audioSource.Play();
        yield return new WaitForSeconds(audioSource.clip.length);
        audioSources.Enqueue(audioSource);
        audioSource.gameObject.SetActive(false);
    }

    IEnumerator LoadAndPlayMusic(Music music, float transitionTime)
    {

        string musicPath = $"Music/{music}";

        ResourceRequest request = Resources.LoadAsync<AudioClip>(musicPath);

        yield return request;

        AudioClip clip = request.asset as AudioClip;

        if (clip == null)
        {
            Debug.LogError($"Failed to load Music at path: {musicPath}", this);
            yield break;
        }

        musicTransition = true;

        AudioSource from = switched ? this.transitionMusicSource : this.musicAudioSource;
        AudioSource to = switched ? this.musicAudioSource : this.transitionMusicSource;
        switched = !switched;

        to.enabled = true;
        to.clip = clip;
        to.Play();

        float elapsed = 0.0f;

        while (elapsed < transitionTime)
        {
            float t = elapsed / transitionTime;
            from.volume = 1 - t;
            to.volume = t;
            elapsed += Managers.game.UnscaledDeltaTime;
            yield return null;
        }
        to.volume = 1.0f;
        from.volume = 0.0f;
        from.enabled = false;
        musicTransition = false;

    }


    AudioMixerGroup GetAudioMixer(AudioType audioType)
    {
        int index = (int)audioType;

        if (index >= this.mixerGroupTypes.Length)
        {
            Debug.LogWarning($"AudioMixerGroup not found for type: {audioType}, using Master group");
            index = 0;
        }

        return this.mixerGroupTypes[index];
    }

}