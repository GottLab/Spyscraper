using System;
using UnityEngine;
using UnityEngine.Events;

public class DialogueHandler : MonoBehaviour
{

    [Serializable]
    public struct DialogueInstance
    {
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
        NextDialogue();
    }

    private void NextDialogue()
    {
        if (!random)
        {
            this.currentDialogueIndex++;
        }

        if (this.currentDialogueIndex >= this.dialogueInstances.Length)
        {
            return;
        }

        DialogueInstance dialogueInstance = this.dialogueInstances[this.currentDialogueIndex];

        Managers.Talk.StartDialogue(dialogueInstance.dialogue, () =>
        {
            if (!random)
            {
                NextDialogue();
            }
            dialogueInstance.OnDialogueEnd?.Invoke();
        });
    }
}
