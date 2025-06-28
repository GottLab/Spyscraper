using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DialogueHandler : MonoBehaviour
{

    [Serializable]
    public class EventDictionary : SerializableDictionary<string, UnityEvent>
    {
        public override string GetNewKey(string key)
        {
            return key + "_new";
        }
    };

    [Serializable]
    public struct DialogueInstance
    {
        [Tooltip("Time to wait to play this dialogue")]
        public float delay;
        public Dialogue dialogue;
        public EventDictionary dialogueEvents;
    }


    [SerializeField, Tooltip("Play one random dialogue from the list instead of being sequential")]
    private bool random;

    [SerializeField]
    private DialogueInstance[] dialogueInstances;

    private int currentDialogueIndex;

    public void StartDialogue()
    {
        currentDialogueIndex = random ? UnityEngine.Random.Range(0, this.dialogueInstances.Length) : -1;
        Managers.Talk.StartCoroutine(NextDialogue());
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

        Managers.Talk.StartDialogue(dialogueInstance.dialogue, OnCharacterDialogueEnd: (charDialogue) =>
        {
            //when dialogueline has an action call the corresponding event
            if (charDialogue.eventInfo.action.Length > 0)
                CallDialogueEvent(dialogueInstance, charDialogue.eventInfo.action, required: true);
                
        }, OnDialogueEnd: () =>
        {
            if (!random)
            {
                Managers.Talk.StartCoroutine(NextDialogue());
            }
            //call end event when dialogue ends
            CallDialogueEvent(dialogueInstance, "End", required: false);
        });
    }

    //calls the dialogue event to make something happen when "eventName" happens
    void CallDialogueEvent(DialogueInstance dialogueInstance, string eventName, bool required = true)
    {
        if (dialogueInstance.dialogueEvents.TryGetValue(eventName, out UnityEvent ev))
        {
            ev?.Invoke();
        }
        else if (required)
        {
            Debug.LogWarning($"Event {eventName} not found");
        }
    }


}
