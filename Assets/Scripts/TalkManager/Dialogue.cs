using System;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// A CharacterDialogue describes a set of lines said by character
/// </summary>
[Serializable]
public struct CharacterDialogue
{
    public CharacterData character;
    public List<string> lines;
}

/// <summary>
/// A Dialogue is a set of CharacterDialogue
/// </summary>
[CreateAssetMenu(fileName = "Dialogue", menuName = "Scriptable Objects/Dialogue")]
public class Dialogue : ScriptableObject
{
    public List<CharacterDialogue> dialogueLines;
}
