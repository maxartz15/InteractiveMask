using UnityEditor;
using UnityEngine;

namespace TAO.InteractiveMask.Editor
{
    [CustomPropertyDrawer(typeof(Mask))]
    public class MaskDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
			// Using BeginProperty / EndProperty on the parent property means that
			// prefab override logic works on the entire property.
			EditorGUI.BeginProperty(position, label, property);

			position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

			int indent = EditorGUI.indentLevel;
			Color color = GUI.color;

			EditorGUI.indentLevel = 0;

			float width = position.width / 4;
			Rect rectR = new Rect(position.x + (width * 0), position.y, width, position.height);
			Rect rectG = new Rect(position.x + (width * 1), position.y, width, position.height);
			Rect rectB = new Rect(position.x + (width * 2), position.y, width, position.height);
			Rect rectA = new Rect(position.x + (width * 3), position.y, width, position.height);

			GUI.color = Color.red;
			EditorGUI.PropertyField(rectR, property.FindPropertyRelative("r"), new GUIContent(""));
			GUI.color = Color.green;
			EditorGUI.PropertyField(rectG, property.FindPropertyRelative("g"), new GUIContent(""));
			GUI.color = Color.cyan;
			EditorGUI.PropertyField(rectB, property.FindPropertyRelative("b"), new GUIContent(""));
			GUI.color = Color.gray;
			EditorGUI.PropertyField(rectA, property.FindPropertyRelative("a"), new GUIContent(""));

			GUI.color = color;
			EditorGUI.indentLevel = indent;

			EditorGUI.EndProperty();
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			return base.GetPropertyHeight(property, label);
		}
	}
}
