using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Unity.VisualScripting;
using UnityEngine;

public class AudioManager : MonoBehaviour, IGameManager
{
    private class Settings : ISaveData
    {
        public readonly bool isMuted = false;
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


    public ManagerStatus status => ManagerStatus.Started;


    public static event Action<AudioType, Vector3> OnSoundPlay;
    public static event Action<AudioType, float> OnVolumeChange;

    [SerializeField, Tooltip("Audio Source used to play music")]
    private TypeAudioSource musicAudioSouce;

    [SerializeField, Tooltip("Audio Source used to play non-spatial ui sounds")]
    private TypeAudioSource uiAudioSouce;

    private Settings audioSettings = new();

    private Queue<TypeAudioSource> audioSources = new();

    public void Startup()
    {
        SaveManager.saveManager.Load(this.audioSettings);

        foreach (AudioType audioType in Enum.GetValues(typeof(AudioType)))
        {
            OnVolumeChange?.Invoke(audioType, this.audioSettings.audioVolume[audioType]);
        }
    }

    public void SetVolume(AudioType audioType, float volume)
    {
        if (volume < 0.0f || volume > 1.0f)
        {
            Debug.LogWarning("Invalid Volume Value it must be between 0 and 1 inclusive");
            return;
        }

        this.audioSettings.audioVolume[audioType] = volume;
        OnVolumeChange?.Invoke(audioType, volume);

        if (audioType == AudioType.Master)
        {
            AudioListener.volume = volume;
        }

        SaveManager.saveManager.Save(this.audioSettings);
    }

    public float GetVolume(AudioType audioType)
    {
        return this.audioSettings.audioVolume[audioType];
    }


    public void PlayMusic(Music music)
    {
        StartCoroutine(LoadAndPlayMusic(music));
    }


    public void PlayUISound(AudioClip audioClip, float volume = 1.0f, AudioType audioType = AudioType.Master)
    {
        this.uiAudioSouce.Volume = volume;
        this.uiAudioSouce.Source.PlayOneShot(audioClip, this.audioSettings.audioVolume[audioType]);
    }

    public void PlayClipAtPoint(AudioClip audioClip, Vector3 position, float volume = 1.0f, AudioType audioType = AudioType.Master)
    {
        TypeAudioSource audioSource;
        if (this.audioSources.Count > 0)
        {
            audioSource = this.audioSources.Dequeue();
        }
        else
        {
            audioSource = new GameObject().AddComponent<AudioSource>().AddComponent<TypeAudioSource>();
            audioSource.AudioType = audioType;
            audioSource.Source.spatialize = true;
            audioSource.Source.spatialBlend = 1.0f;
            audioSource.Source.loop = false;
            audioSource.gameObject.hideFlags = HideFlags.HideInHierarchy;
        }
        audioSource.gameObject.SetActive(true);
        audioSource.Source.clip = audioClip;
        audioSource.Source.volume = volume;
        audioSource.transform.position = position;

        OnSoundPlay?.Invoke(audioType, position);

        StartCoroutine(PlayAudioSource(audioSource));
    }


    IEnumerator PlayAudioSource(TypeAudioSource audioSource)
    {
        audioSource.Source.Play();
        yield return new WaitForSeconds(audioSource.Source.clip.length);
        audioSources.Enqueue(audioSource);
        audioSource.gameObject.SetActive(false);
    }

    IEnumerator LoadAndPlayMusic(Music music)
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

        this.musicAudioSouce.Source.clip = clip;

        this.musicAudioSouce.Source.Play();

    }


    public bool soundMute {
        get {return AudioListener.pause;}
        set {AudioListener.pause = value;}
    }

}