using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static DialogueLoader;

[RequireComponent(typeof(AudioSource))]
public class TalkManager : MonoBehaviour
{
    public static TalkManager Instance { get; private set; }

    [SerializeField]
    private CanvasGroup canvasGroup;


    [SerializeField]
    private Image mugShot;

    private Animator mugShotAnimator;

    [SerializeField]
    private TextMeshProUGUI dialogueText;

    [SerializeField]
    private TextMeshProUGUI characterNameText;

    private AudioSource audioSource;


    public float charactersPerSecond = 30f; // Speed of typing

    public float minReadTimePerChar = 0.035f; // Used to estimate read time

    public float fadeDuration = 0.7f; // Used to estimate read time

    private Emotion emotion = Emotion.NORMAL;

    private Coroutine currentDialogueCoroutine = null;

    public Dictionary<string, string> characters = new();

    void Awake()
    {
        if (Instance != null && Instance != this) 
        { 
            Destroy(this); 
        } 
        else 
        { 
            Instance = this;
            this.mugShotAnimator = this.mugShot.GetComponent<Animator>();
            this.audioSource = this.GetComponent<AudioSource>();
            DontDestroyOnLoad(this.gameObject);
        } 
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            StartDialogue("intro2");
        }
    }

    private IEnumerator FadeTextBox(bool fadeIn)
    {
        float startAlpha = fadeIn ? 0.0f : 1.0f;
        float targetAlpha = 1 - startAlpha;
        float timeElapsed = 0f;

        if(Mathf.Approximately(canvasGroup.alpha, targetAlpha))
            yield break;
        

        while (timeElapsed < fadeDuration)
        {
            timeElapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, timeElapsed / fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = targetAlpha;
    }

    void ResetDialogueData()
    {
        this.dialogueText.text = "";
        this.emotion = Emotion.NORMAL;
        if(this.mugShotAnimator.runtimeAnimatorController != null)
        {
            UpdateEmotion(this.emotion);
            this.mugShotAnimator.Play("Base Layer.NORMAL", 0, 0f);  //this is used to avoid blending when switching characters
        }
    }


    public void StartDialogue(string dialogueFile, Action OnDialogueEnd = null)
    {
        this.ResetDialogueData();
        if(currentDialogueCoroutine != null)
        {
            StopAllCoroutines();
            this.currentDialogueCoroutine = null;
        }
        Dialogue dialogue = DialogueLoader.ReadDialogue(dialogueFile);
        if(dialogue != null)
        {
            this.currentDialogueCoroutine = StartCoroutine(StartDialogue(dialogue, OnDialogueEnd));
        }
    }

    private void UpdateEmotion(Emotion emotion)
    {
        //mugShotAnimator.Play("Base Layer." + emotion.ToString());
        mugShotAnimator.SetInteger("Emotion", (int)emotion);
    }


    private IEnumerator StartDialogue(Dialogue dialogue, Action OnDialogueEnd = null)
    {

        yield return StartCoroutine(FadeTextBox(true)); 
        
        CharacterData prevCharacterData = null;
        
        for (int i = 0; i < dialogue.Lines.Count; i++)
        {   
            dialogueText.text = "";
            DialogueLine line = dialogue.Lines[i];
            
            if(prevCharacterData != line.CharacterData)
            {
                this.characterNameText.text = line.CharacterData.characterName;
                ResetDialogueData();
            }
            prevCharacterData = line.CharacterData;

            mugShotAnimator.runtimeAnimatorController = line.CharacterData.controller;
            yield return StartCoroutine(line.TypeText(this.charactersPerSecond, this.minReadTimePerChar, (c) => {
                dialogueText.text += c;
                if(line.CharacterData.talkAudioClip != null && c != ' ')
                {
                    this.audioSource.Stop();
                    this.audioSource.clip = line.CharacterData.talkAudioClip;
                    this.audioSource.Play();
                }
            }, UpdateEmotion));
            
        }
        OnDialogueEnd?.Invoke();
        yield return StartCoroutine(FadeTextBox(false)); 
    }

    public float EstimateReadingTime(string text)
    {
        return Mathf.Clamp(text.Length * minReadTimePerChar, 1.5f, 10f); // Clamp for practicality
    } 
}
