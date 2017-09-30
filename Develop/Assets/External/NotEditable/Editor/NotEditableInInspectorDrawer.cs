using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomPropertyDrawer(typeof(NotEditableInInspector))]
public class NotEditableInInspectorDrawer : PropertyDrawer 
{
	public override void OnGUI(Rect pos, SerializedProperty sp, GUIContent lbl)
	{
		EditorGUI.BeginDisabledGroup(true);
		EditorGUI.PropertyField(pos, sp, lbl, true);
		EditorGUI.EndDisabledGroup();
	}
	
	public override float GetPropertyHeight(SerializedProperty sp, GUIContent lbl)
	{
		float height = EditorGUI.GetPropertyHeight(sp, lbl, true);
		return height;
	}
}
