using System;
using System.Globalization;
using System.Text.RegularExpressions;
using UnityEngine;

public enum Emotion
{
    NORMAL,
    ANGRY,
    HAPPY,
    SAD
}

public class DialogueInfo
{
    public Emotion emotion;
    public float charactersPerSecondMultiplier;
    public float pauseTime;

    public void Reset()
    {
        emotion = default;
        charactersPerSecondMultiplier = 1.0F;
        pauseTime = 0.0F;
    }
}

public class DialogueModifier
{

    private static bool ApplyDialogueModifier(char character, string value, DialogueInfo dialogueInfo)
    {
        try
        {
            switch (character)
            {
                case 'p':
                    float pauseTime = float.Parse(value, NumberStyles.Float, CultureInfo.InvariantCulture);
                    dialogueInfo.pauseTime = pauseTime;
                    break;
                case 's':
                    float multiplier = float.Parse(value, NumberStyles.Float, CultureInfo.InvariantCulture);
                    dialogueInfo.charactersPerSecondMultiplier = multiplier;
                    break;
                case 'e':
                    Emotion emotion = Enum.Parse<Emotion>(value, ignoreCase: true);
                    dialogueInfo.emotion = emotion;
                    break;
                default:
                    return false; // Unknown modifier character
            }
        }
        catch (FormatException)
        {
            return false;
        }
        return true;
    }


    public static bool ApplyModifier(string text, DialogueInfo dialogueInfo, ref int index)
    {
        if (index >= text.Length)
            return false;

        char c = text[index];

        if (c == '|')
        {
            int length = Math.Min(12, text.Length - index);
            string input = text.Substring(index, length);
            Match match = Regex.Match(input, @"\|(\w{1})([\w\.]+)\|\s*");
            if (match.Success)
            {
                string modifierChar = match.Groups[1].Value;
                string result = match.Groups[2].Value;

                if (ApplyDialogueModifier(modifierChar[0], result, dialogueInfo))
                {
                    index += result.Length + 3;
                    return true;
                }
                else
                {
                    Debug.LogError($"Failed parsing modifier: {modifierChar}, text: {text}, index: {index}, value: {result}");
                }

            }
        }
        return false;
    }
}



