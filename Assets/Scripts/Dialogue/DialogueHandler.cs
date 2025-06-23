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

    private int currentDialogueIndex = -1;

    public void StartDialogue()
    {
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
