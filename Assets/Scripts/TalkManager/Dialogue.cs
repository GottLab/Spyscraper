using System;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// A CharacterDialogue describes a set of lines said by character
/// </summary>
[Serializable]
public struct CharacterDialogue
{
    [Serializable]
    public struct Event
    {
        [Tooltip("Event to wait before going forward")]
        public GameManager.GameEvent gameEvent;

        [Tooltip("Action Name used to make something happen")]
        public string action;
    }

    public CharacterData character;
    public List<string> lines;
    public Event eventInfo;
}

/// <summary>
/// A Dialogue is a set of CharacterDialogue
/// </summary>
[CreateAssetMenu(fileName = "Dialogue", menuName = "Scriptable Objects/Dialogue")]
public class Dialogue : ScriptableObject
{
    public List<CharacterDialogue> dialogueLines;
}
