using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

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