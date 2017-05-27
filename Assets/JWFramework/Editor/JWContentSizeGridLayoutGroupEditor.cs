using UnityEngine;
using UnityEditor;
using System.Collections;

[CanEditMultipleObjects]
[CustomEditor (typeof(JWContentSizeGridLayoutGroup))]
public class JWContentSizeGridLayoutGroupEditor : UnityEditor.UI.GridLayoutGroupEditor
{
	SerializedProperty m_HorizontalFit;
	SerializedProperty m_VerticalFit;

	protected override void OnEnable ()
	{
		base.OnEnable ();
		m_HorizontalFit = serializedObject.FindProperty ("m_HorizontalFit");
		m_VerticalFit = serializedObject.FindProperty ("m_VerticalFit");
	}

	public override void OnInspectorGUI ()
	{
		base.OnInspectorGUI ();
		
		serializedObject.Update ();
		EditorGUILayout.PropertyField (m_HorizontalFit, true);
		EditorGUILayout.PropertyField (m_VerticalFit, true);
		serializedObject.ApplyModifiedProperties ();
	}
}
