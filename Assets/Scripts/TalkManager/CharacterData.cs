using UnityEngine;

[CreateAssetMenu(fileName = "CharacterData", menuName = "Scriptable Objects/CharacterData")]
public class CharacterData : ScriptableObject
{
    public string characterName;
    public AnimatorOverrideController controller;

    public AudioClip talkAudioClip;

    private const int maxLength = 15;

    void OnValidate()
    {
        if (characterName != null && characterName.Length > maxLength)
        {
            Debug.LogWarning($"Character name is too long. Truncating to {maxLength} characters.");
            characterName = characterName.Substring(0, maxLength);
        }

        if(controller == null)
        {
            Debug.LogWarning($"Character Data Controller: {this.name} is empty");
        }

    }
}
