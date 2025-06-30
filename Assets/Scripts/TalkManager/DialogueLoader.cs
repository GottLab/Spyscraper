using System;
using UnityEngine;

public class DialogueLoader 
{

    
    public static Dialogue ReadDialogue(String dialogue)
    {
        Dialogue dialogueFile = Resources.Load<Dialogue>($"Dialogues/{dialogue}");

        if (dialogueFile != null)
        {
            return dialogueFile;
        }

        Debug.LogError($"Dialogue \"{dialogue}\" file not found!");
        return null;
    }

}