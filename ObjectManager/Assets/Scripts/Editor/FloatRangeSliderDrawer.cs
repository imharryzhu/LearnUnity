using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(FloatRangeSliderAttribute))]
public class FloatRangeSliderDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        int originalIndentLevel = EditorGUI.indentLevel;

        EditorGUI.BeginProperty(position, label, property);

        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
        EditorGUI.indentLevel = 0;

        SerializedProperty min = property.FindPropertyRelative("min");
        SerializedProperty max = property.FindPropertyRelative("max");
        float minValue = min.floatValue;
        float maxValue = max.floatValue;
        position.width /= 3;
        minValue = EditorGUI.FloatField(position, minValue);
        position.x += position.width;
        FloatRangeSliderAttribute limit = attribute as FloatRangeSliderAttribute;
        EditorGUI.MinMaxSlider(position, ref minValue, ref maxValue, limit.Min, limit.Max);
        position.x += position.width;
        maxValue = EditorGUI.FloatField(position, maxValue);
        min.floatValue = minValue;
        max.floatValue = maxValue;
        EditorGUI.EndProperty();

        EditorGUI.indentLevel = originalIndentLevel;
    }
}
