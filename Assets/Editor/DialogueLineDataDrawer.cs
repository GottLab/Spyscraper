using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[CustomPropertyDrawer(typeof(CharacterDialogue))]
public class DialogueLineDataDrawer : PropertyDrawer
{
    private ReorderableList reorderableList;

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        var linesProp = property.FindPropertyRelative("lines");

        if (reorderableList == null || reorderableList.serializedProperty != linesProp)
        {
            SetupReorderableList(linesProp);
        }

        float height = EditorGUI.GetPropertyHeight(property.FindPropertyRelative("character"));
        height += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("gameEvent"));
        height += reorderableList.GetHeight();
        return height + 6f;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var characterProp = property.FindPropertyRelative("character");
        var linesProp = property.FindPropertyRelative("lines");
        var gameEventProp = property.FindPropertyRelative("gameEvent");

        if (reorderableList == null || reorderableList.serializedProperty != linesProp)
        {
            SetupReorderableList(linesProp);
        }

        position.height = EditorGUI.GetPropertyHeight(characterProp);
        EditorGUI.PropertyField(position, characterProp);

        
        position.y += position.height + 2f;
        position.height = EditorGUI.GetPropertyHeight(gameEventProp);
        EditorGUI.PropertyField(position, gameEventProp, new GUIContent("Required Action"));

        position.y += position.height + 2;
        reorderableList.DoList(position);

    }

    private void SetupReorderableList(SerializedProperty linesProp)
    {
        reorderableList = new ReorderableList(linesProp.serializedObject, linesProp, true, true, true, true);

        reorderableList.drawHeaderCallback = rect =>
        {
            EditorGUI.LabelField(rect, "Lines");
        };

        reorderableList.drawElementCallback = (rect, index, isActive, isFocused) =>
        {
            SerializedProperty element = linesProp.GetArrayElementAtIndex(index);
            rect.y += 2;
            // Get current value and trim if needed
            string currentValue = element.stringValue;
            int maxChars = 100;

            // Draw the text field and limit input length
            string newValue = EditorGUI.TextField(
                new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),
                GUIContent.none,
                currentValue
            );

            // Enforce the character limit
            if (newValue.Length > maxChars)
            {
                newValue = newValue[..maxChars];
            }

            // Apply the updated value
            element.stringValue = newValue;
        };
    }
}
