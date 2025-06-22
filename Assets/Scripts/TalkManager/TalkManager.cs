using System;
using System.Collections;
using UnityEngine;
using static GameManager;

public class TalkManager : MonoBehaviour, IGameManager
{

    public ManagerStatus status => ManagerStatus.Started;

    public float charactersPerSecond = 30f; // Speed of typing

    public float minReadTimePerChar = 0.035f; // Used to estimate read time

    public float fadeDuration = 0.7f; // Used to estimate read time

    private Coroutine currentDialogueCoroutine = null;


    public delegate void DialogueFade(float fadeValue, bool fadeIn);
    public static DialogueFade OnDialogueFade;


    public static Action<Dialogue> OnDialogueStart;
    public static Action<CharacterDialogue> OnCharacterDialogueStart;
    public delegate void CharacterType(CharacterDialogue dialogueLine, DialogueInfo info, char character);
    public static CharacterType OnCharacterType;


    private Action<GameEvent> currentGameEventToWait;
    

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.L))
        {
            print("START!");
            StartDialogue("intro");
        }
    }

    private IEnumerator FadeTextBox(bool fadeIn)
    {
        float startAlpha = fadeIn ? 0.0f : 1.0f;
        float targetAlpha = 1 - startAlpha;
        float timeElapsed = 0f;

        while (timeElapsed < fadeDuration)
        {
            timeElapsed += Time.deltaTime;
            OnDialogueFade?.Invoke(Mathf.Lerp(startAlpha, targetAlpha, timeElapsed / fadeDuration), fadeIn);
            yield return null;
        }
        OnDialogueFade?.Invoke(targetAlpha, fadeIn);
    }

    public void StartDialogue(string dialogueFile, Action OnDialogueEnd = null)
    {
        if (currentDialogueCoroutine != null)
        {
            if (this.currentGameEventToWait != null)
            {
                GameManager.OnGameEvent -= this.currentGameEventToWait;
            }
            StopAllCoroutines();
            this.currentDialogueCoroutine = null;
        }
        Dialogue dialogue = DialogueLoader.ReadDialogue(dialogueFile);
        if (dialogue != null)
        {
            this.currentDialogueCoroutine = StartCoroutine(StartDialogue(dialogue, OnDialogueEnd));
        }
    }
    
    private IEnumerator TypeText(CharacterDialogue characterDialogue, string line, DialogueInfo dialogueInfo)
    {

        float totalDelay = 0;
        int totalCharacters = 0;
        for (int i = 0; i < line.Length; i++)
        {
            DialogueModifier.ApplyModifier(line, dialogueInfo, ref i);
            yield return new WaitForSeconds(dialogueInfo.pauseTime);
            dialogueInfo.pauseTime = 0.0F;
           
            float delay = 1f / charactersPerSecond;
            delay *= dialogueInfo.charactersPerSecondMultiplier;
            totalDelay += delay;
            yield return new WaitForSeconds(delay);

            if (i < line.Length)
                OnCharacterType?.Invoke(characterDialogue, dialogueInfo, line[i]);
            totalCharacters++;
        }

        yield return new WaitForSeconds(Mathf.Max(totalCharacters * minReadTimePerChar - totalDelay, 1.0f));
    }


    private IEnumerator StartDialogue(Dialogue dialogue, Action OnDialogueEnd = null)
    {
        OnDialogueStart?.Invoke(dialogue);
        yield return StartCoroutine(FadeTextBox(true));

        DialogueInfo dialogueInfo = new();

        foreach (CharacterDialogue dialogueLine in dialogue.dialogueLines)
        {

            // ----- Game Event Section -----
            //this section handles listening to game events
            bool complete = true;
            if (dialogueLine.gameEvent != GameEvent.None)
            {
                complete = false;
                //when we receive a specific game event then when mark complete to true
                Action<GameEvent> handleGameEvent = (gameEvent) =>
                {
                    if (gameEvent == dialogueLine.gameEvent)
                        complete = true;
                };
                this.currentGameEventToWait = handleGameEvent;
                GameManager.OnGameEvent += handleGameEvent;
                // ------------
            }


            dialogueInfo.Reset();
            OnCharacterDialogueStart?.Invoke(dialogueLine);

            foreach (string line in dialogueLine.lines)
            {
                OnCharacterDialogueStart?.Invoke(dialogueLine);
                yield return StartCoroutine(TypeText(dialogueLine, line, dialogueInfo));
            }

            yield return new WaitUntil(() => complete);
            
            if (currentGameEventToWait != null)
                GameManager.OnGameEvent -= this.currentGameEventToWait;
        }

        


        OnDialogueEnd?.Invoke();
        yield return StartCoroutine(FadeTextBox(false));
    }

    public float EstimateReadingTime(string text)
    {
        return Mathf.Clamp(text.Length * minReadTimePerChar, 1.5f, 10f); // Clamp for practicality
    }

    public void Startup()
    {
    }
}
