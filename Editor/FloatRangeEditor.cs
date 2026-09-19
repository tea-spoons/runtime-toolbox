
namespace TeaSpoons.RuntimeToolbox.Editor
{
    using UnityEngine;
    using UnityEditor;

    [CustomPropertyDrawer(typeof(FloatRange))]
    public class NumberRangeEditor : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            var leftPosition = new Rect(position.x,
                                        position.y,
                                        position.width / 2 - 10,
                                        position.height);
            var dashPosition = new Rect(position.x + position.width / 2 - 5,
                                        position.y,
                                        20,
                                        position.height);
            var rightPosition = new Rect(position.x + position.width / 2 + 10,
                                         position.y,
                                         position.width / 2 - 10,
                                         position.height);

            EditorGUI.PropertyField(leftPosition, property.FindPropertyRelative("Min"), GUIContent.none);
            GUI.Label(dashPosition, "-");
            EditorGUI.PropertyField(rightPosition, property.FindPropertyRelative("Max"), GUIContent.none);

            EditorGUI.indentLevel = indent;

            EditorGUI.EndProperty();
        }
    }

}
