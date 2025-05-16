using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public record Dialogue(List<DialogueLine> Lines);

public delegate bool StringOutFunc<TOut>(string input, out TOut result);


public enum Emotion
{
    NORMAL,
    ANGRY
}

public record DialogueLine(string Line, CharacterData CharacterData){


    private static readonly DialogueModifier<float> FLOAT_MODIFIER = new ((string s, out float v) => float.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out v));
    private static readonly DialogueModifier<string> STRING_MODIFIER = new ((string s, out string v) => {
        v = s;
        return true;
    });

    private static readonly DialogueModifier<Emotion> EMOTION_MODIFIER = new ((string s, out Emotion v) => Enum.TryParse(s, ignoreCase: true, out v));


    public IEnumerator TypeText(float charactersPerSecond, float minReadTimePerChar, Action<char> onType, Action<Emotion> onEmotionChange)
    {

        float charactersPerSecondMultiplier = 1.0f;
        float totalDelay = 0;
        
        int totalCharacters = 0;
        for (int i = 0;i < Line.Length; i++)
        {
            float pauseTime = 0.0f;
            FLOAT_MODIFIER.GetValue('p', Line, ref i, ref pauseTime);
            yield return new WaitForSeconds(pauseTime);
            FLOAT_MODIFIER.GetValue('s', Line, ref i, ref charactersPerSecondMultiplier);
            Emotion emotion = Emotion.NORMAL;
            if(EMOTION_MODIFIER.GetValue('e', Line, ref i, ref emotion))
                onEmotionChange(emotion);
            
            float delay = 1f / charactersPerSecond;
            delay *= charactersPerSecondMultiplier;
            totalDelay += delay;
            yield return new WaitForSeconds(delay);
            
            if(i < Line.Length)
                onType(Line[i]);
            totalCharacters++;  
        }
        
        yield return new WaitForSeconds(Mathf.Max(totalCharacters * minReadTimePerChar - totalDelay, 1.0f));
    }
}

public sealed record DialogueModifier<T>(StringOutFunc<T> fromString)
{

    public bool GetValue(char modifierChar, string text, ref int index, ref T value)
    {
        if(index >= text.Length)
            return false;

        char c = text[index];
        
        if(c == '|')
        {
            int length = Math.Min(12, text.Length - index);
            string input = text.Substring(index,length);
            Match match = Regex.Match(input, $@"\|{modifierChar}([\w\.]+)\|\s*");
            if (match.Success)
            {
                string result = match.Groups[1].Value;
                
                if(fromString(result, out value))
                {   
                    index += result.Length + 3;
                    return true;
                }
                else
                {
                    Debug.LogError($"Failed parsing modifier: {modifierChar}, text: {text}, index: {index}, value: {value}");
                }

            }
        }
        return false;
    }
}
