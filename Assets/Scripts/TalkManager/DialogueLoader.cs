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
        TextAsset dialogueFile = Resources.Load<TextAsset>($"Dialogues/{dialogue}");

        if (dialogueFile != null)
        {
            string dialogueText = dialogueFile.text;
            Resources.UnloadAsset(dialogueFile);
            
            string[] lines = dialogueText.Split(new []{"\n", "\r\n"}, System.StringSplitOptions.None);
            List<DialogueLine> dialogueLines = new();

            CharacterData currentCharacterData = null;

            foreach(string line in lines)
            {
                Match characterNameMatch = Regex.Match(line, @"\[(\w+)\]");
                if(characterNameMatch.Success)
                {
                    string characterName = characterNameMatch.Groups[1].Value;
                    currentCharacterData = Resources.Load<CharacterData>($"Characters/{characterName}");
                    if(currentCharacterData == null)
                    {
                        Debug.LogError($"Dialogue \"{dialogue}\", Character \"{characterName}\" does not exist.");
                        return null;
                    }
                }
                else
                {
                    dialogueLines.Add(new DialogueLine(line, currentCharacterData));
                }
                if(currentCharacterData == null)
                {
                    Debug.LogError($"Dialogue \"{dialogue}\" must define an initial character!");
                    return null;
                }
            }
            
            return new Dialogue(dialogueLines);
        }
        else
        {
            Debug.LogError($"Dialogue \"{dialogue}\" file not found!");
        }
        return null;
    }

}