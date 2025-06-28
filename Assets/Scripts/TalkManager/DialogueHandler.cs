using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class DialogueHandler : MonoBehaviour
{

    [Serializable]
    public struct DialogueInstance
    {
        [Tooltip("Time to wait to play this dialogue")]
        public float delay;
        public Dialogue dialogue;
        public UnityEvent OnDialogueEnd;
    }


    [SerializeField, Tooltip("Play one random dialogue from the list instead of being sequential")]
    private bool random;

    [SerializeField]
    private DialogueInstance[] dialogueInstances;

    private int currentDialogueIndex;

    public void StartDialogue()
    {
        currentDialogueIndex = random ? UnityEngine.Random.Range(0, this.dialogueInstances.Length) : -1;
        StartCoroutine(NextDialogue());
    }

    private IEnumerator NextDialogue()
    {
        if (!random)
        {
            this.currentDialogueIndex++;
        }

        if (this.currentDialogueIndex >= this.dialogueInstances.Length)
        {
            yield break;
        }

        DialogueInstance dialogueInstance = this.dialogueInstances[this.currentDialogueIndex];

        yield return new WaitForSeconds(dialogueInstance.delay);

        Managers.Talk.StartDialogue(dialogueInstance.dialogue, () =>
        {
            if (!random)
            {
                StartCoroutine(NextDialogue());
            }
            dialogueInstance.OnDialogueEnd?.Invoke();
        });
    }


}
