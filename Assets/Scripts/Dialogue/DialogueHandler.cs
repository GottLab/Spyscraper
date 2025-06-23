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

    [SerializeField]
    private DialogueInstance[] dialogueInstances;

    private int currentDialogueIndex;

    public void StartDialogue()
    {
        currentDialogueIndex = -1;
        NextDialogue();
    }

    private void NextDialogue()
    {
        this.currentDialogueIndex++;

        if (this.currentDialogueIndex >= this.dialogueInstances.Length)
        {
            return;
        }

        DialogueInstance dialogueInstance = this.dialogueInstances[this.currentDialogueIndex];

        Managers.Talk.StartDialogue(dialogueInstance.dialogue, () =>
        {
            NextDialogue();
            dialogueInstance.OnDialogueEnd?.Invoke();
        });
    }
}
