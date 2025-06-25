using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using static GameManager;

public class TalkManager : MonoBehaviour, IGameManager
{

    public ManagerStatus status => ManagerStatus.Started;

    public float charactersPerSecond = 30f; // Speed of typing

    public float minReadTimePerChar = 0.035f; // Used to estimate read time

    public float fadeDuration = 0.7f;

    private Coroutine currentDialogueCoroutine = null;


    public delegate void DialogueFade(float fadeValue, bool fadeIn);
    public static DialogueFade OnDialogueFade;


    public static Action<Dialogue> OnDialogueStart;
    public static Action<CharacterDialogue> OnCharacterDialogueStart;
    public delegate void CharacterType(CharacterDialogue dialogueLine, DialogueInfo info, char character);
    public static CharacterType OnCharacterType;

    public static event Action OnDialogueEnd;

    private Action<GameEvent> currentGameEventToWait;

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

    public float CharactersPerSecond
    {
        get => this.charactersPerSecond;
        set => this.charactersPerSecond = CharactersPerSecond;
    }

    public void StartDialogue(string dialogueFile, Action OnDialogueEnd = null)
    {
        Dialogue dialogue = DialogueLoader.ReadDialogue(dialogueFile);
        this.StartDialogue(dialogue, OnDialogueEnd);
    }

    public void StartDialogue(Dialogue dialogue, Action OnDialogueEnd = null)
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
        //Dialogue dialogue = DialogueLoader.ReadDialogue(dialogueFile);
        if (dialogue != null)
        {
            this.currentDialogueCoroutine = StartCoroutine(StartDialogueCoroutine(dialogue, OnDialogueEnd));
        }
    }

    private IEnumerator TypeText(CharacterDialogue characterDialogue, string line, DialogueInfo dialogueInfo)
    {
        //this is used to avoid to skip when pressing enter of the previous line
        //we wait one frame to be sure that the skip key is no longer pressed
        yield return null;

        float totalDelay = 0;
        int totalCharacters = 0;
        //should it skip the rest of the line?
        bool skip = false;
        for (int i = 0; i < line.Length; i++)
        {

            DialogueModifier.ApplyModifier(line, dialogueInfo, ref i);
            if (!skip && dialogueInfo.pauseTime > 0.0f)
                yield return WaitSecondsOrSkip(dialogueInfo.pauseTime, () => skip = true);
            dialogueInfo.pauseTime = 0.0F;

            float delay = 1f / charactersPerSecond;
            delay *= dialogueInfo.charactersPerSecondMultiplier;
            totalDelay += delay;

            if (!skip)
                yield return WaitSecondsOrSkip(delay, () => skip = true);

            if (i < line.Length)
                OnCharacterType?.Invoke(characterDialogue, dialogueInfo, line[i]);
            totalCharacters++;
        }

        //wait one frame to avoid to skip this dialogue and go directly to next since it would happen in the same frame
        yield return null;
    }


    private IEnumerator StartDialogueCoroutine(Dialogue dialogue, Action OnDialogueEnd = null)
    {
        OnDialogueStart?.Invoke(dialogue);
        yield return StartCoroutine(FadeTextBox(true));

        DialogueInfo dialogueInfo = new();

        foreach (CharacterDialogue dialogueLine in dialogue.dialogueLines)
        {

            // ----- Game Event Section -----
            //this section handles listening to game events
            bool gameEventComplete = true;
            if (dialogueLine.gameEvent != GameEvent.None)
            {
                gameEventComplete = false;
                //when we receive a specific game event then when mark complete to true
                Action<GameEvent> handleGameEvent = (gameEvent) =>
                {
                    if (gameEvent == dialogueLine.gameEvent)
                        gameEventComplete = true;
                };
                this.currentGameEventToWait = handleGameEvent;
                GameManager.OnGameEvent += handleGameEvent;

            }
            // ---------------------


            dialogueInfo.Reset();
            OnCharacterDialogueStart?.Invoke(dialogueLine);

            foreach (string line in dialogueLine.lines)
            {
                OnCharacterDialogueStart?.Invoke(dialogueLine);
                yield return StartCoroutine(TypeText(dialogueLine, line, dialogueInfo));

                //only when we don't to wait a certain game event then we continue when pressing the skip button
                if (dialogueLine.gameEvent == GameEvent.None)
                {
                    yield return new WaitUntil(() => IsSkipButtonPressed);
                }
            }
            //here we wait for a certain game event if set
            yield return new WaitUntil(() => gameEventComplete);

            //unhook from gameEvent action when we used it
            if (currentGameEventToWait != null)
                GameManager.OnGameEvent -= this.currentGameEventToWait;
        }




        OnDialogueEnd?.Invoke();
        yield return StartCoroutine(FadeTextBox(false));
        TalkManager.OnDialogueEnd?.Invoke();
    }

    public float EstimateReadingTime(string text)
    {
        return Mathf.Clamp(text.Length * minReadTimePerChar, 1.5f, 10f); // Clamp for practicality
    }

    public void Startup()
    {
    }


    //Wait for time in seconds or skip immediatly if return is pressed
    private IEnumerator WaitSecondsOrSkip(float time, Action OnSkipPressed)
    {
        float timeElapsed = 0.0f;
        bool skip = false;
        while (timeElapsed < time && !skip)
        {
            timeElapsed += Time.deltaTime;

            if (IsSkipButtonPressed)
            {
                skip = true;
                OnSkipPressed.Invoke();
            }
            else
                yield return null;
        }
    }

    private bool IsSkipButtonPressed
    {
        get => GameManager.GetKeyDown(KeyCode.Return);
    }
}
