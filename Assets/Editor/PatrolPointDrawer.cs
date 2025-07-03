using Enemy;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(PatrolPoint))]
public class PatrolPointDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        var rotationType = (PatrolPoint.RotationType)property.FindPropertyRelative("rotationType").enumValueIndex;
        int lines = 2; // position + rotationType (always shown)

        switch (rotationType)
        {
            case PatrolPoint.RotationType.FIXED:
                lines += 1; // fixedAngle
                break;
            case PatrolPoint.RotationType.AROUND:
                lines += 2; // turnAngle + turnWaitTime
                break;
        }

        return lines * EditorGUIUtility.singleLineHeight + (lines - 1) * EditorGUIUtility.standardVerticalSpacing;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        var posProp = property.FindPropertyRelative("position");
        var typeProp = property.FindPropertyRelative("rotationType");
        var angleProp = property.FindPropertyRelative("turnAngle");
        var waitProp = property.FindPropertyRelative("turnWaitTime");
        var fixedProp = property.FindPropertyRelative("fixedAngle");

        float lineHeight = EditorGUIUtility.singleLineHeight;
        float spacing = EditorGUIUtility.standardVerticalSpacing;

        Rect line = new Rect(position.x, position.y, position.width, lineHeight);
        EditorGUI.PropertyField(line, posProp);

        line.y += lineHeight + spacing;
        EditorGUI.PropertyField(line, typeProp);

        var rotationType = (PatrolPoint.RotationType)typeProp.enumValueIndex;

        switch (rotationType)
        {
            case PatrolPoint.RotationType.FIXED:
                line.y += lineHeight + spacing;
                EditorGUI.PropertyField(line, fixedProp);
                break;

            case PatrolPoint.RotationType.AROUND:
                line.y += lineHeight + spacing;
                EditorGUI.PropertyField(line, angleProp);

                line.y += lineHeight + spacing;
                EditorGUI.PropertyField(line, waitProp);
                break;
        }

        EditorGUI.EndProperty();
    }
}
