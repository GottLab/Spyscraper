using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This Component handles the update of all UI regarding the dialogue
/// </summary>
public class DialogueTextUpdate : MonoBehaviour, IAnimationClipSource
{
    [SerializeField]
    private TextMeshProUGUI dialogueText;

    [SerializeField]
    private TextMeshProUGUI dialogueTitleText;

    [SerializeField]
    private Image mugShot;

    private Animator mugShotAnimator;

    [SerializeField]
    private AnimationClip textBoxAnimation;


    void Start()
    {
        this.mugShotAnimator = mugShot.GetComponent<Animator>();
        this.SetVisible(false);

    }

    void OnEnable()
    {
        TalkManager.OnCharacterType += DialogueType;
        TalkManager.OnCharacterDialogueStart += StartCharacterDialogue;
        TalkManager.OnDialogueStart += StartDialogue;
        TalkManager.OnDialogueFade += FadeCanvas;
    }

    void OnDisable()
    {
        TalkManager.OnCharacterType -= DialogueType;
        TalkManager.OnCharacterDialogueStart -= StartCharacterDialogue;
        TalkManager.OnDialogueStart -= StartDialogue;
        TalkManager.OnDialogueFade -= FadeCanvas;
    }

    private void DialogueType(CharacterDialogue dialogueLine, DialogueInfo info, char character)
    {
        dialogueText.text += character;
        this.UpdateEmotion(info.emotion);
    }

    private void StartCharacterDialogue(CharacterDialogue characterDialogue)
    {
        dialogueTitleText.text = characterDialogue.character.name;
        dialogueText.text = "";
        this.mugShotAnimator.runtimeAnimatorController = characterDialogue.character.controller;
        this.UpdateEmotion(Emotion.NORMAL);
        //this is used for forcing the current emotion animation without transitions.
        this.mugShotAnimator.Play("BASE.NORMAL", 0, 0f);

    }

    private void StartDialogue(Dialogue dialogue)
    {
        this.SetVisible(true);
        CharacterDialogue? characterDialogue = dialogue.dialogueLines.FirstOrDefault();

        if (characterDialogue.HasValue)
        {
            StartCharacterDialogue(characterDialogue.Value);
        }
    }

    private void FadeCanvas(float fade, bool fadeIn)
    {
        textBoxAnimation.SampleAnimation(this.gameObject, fade);
    }

    //This method update the current animation based on the Emotion enum
    private void UpdateEmotion(Emotion emotion)
    {
        mugShotAnimator.SetInteger("Emotion", (int)emotion);
    }

    public void GetAnimationClips(List<AnimationClip> results)
    {
        results.Add(this.textBoxAnimation);
    }


    void SetVisible(bool visible)
    {
        this.gameObject.SetActive(visible);
    }

}