using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Reflection;

public static class JWEditorTools
{
	/// <summary>
	/// Draw a distinctly different looking header label
	/// </summary>

	static public bool DrawHeader (string text)
	{
		return DrawHeader (text, text, false, false);
	}

	/// <summary>
	/// Draw a distinctly different looking header label
	/// </summary>

	static public bool DrawHeader (string text, string key, bool forceOn, bool minimalistic)
	{
		bool state = EditorPrefs.GetBool (key, true);
		GUILayout.Space (3f);
		if (!forceOn && !state)
			GUI.backgroundColor = new Color (0.8f, 0.8f, 0.8f);
		GUILayout.BeginHorizontal ();
		GUI.changed = false;
		text = "<b><size=11>" + text + "</size></b>";
		if (state)
			text = "\u25BC " + text;
		else
			text = "\u25BA " + text;
		if (!GUILayout.Toggle (true, text, "dragtab", GUILayout.MinWidth (20f)))
			state = !state;
		if (GUI.changed)
			EditorPrefs.SetBool (key, state);
		GUILayout.Space (2f);
		GUILayout.EndHorizontal ();
		GUI.backgroundColor = Color.white;
		if (!forceOn && !state)
			GUILayout.Space (3f);
		return state;
	}

	static public void SetLabelWidth (float width)
	{
		EditorGUIUtility.labelWidth = width;
	}

	static public void BeginContents ()
	{
		GUILayout.BeginHorizontal ();
		EditorGUILayout.BeginHorizontal ("AS TextArea", GUILayout.MinHeight (10f));
		GUILayout.BeginVertical ();
		GUILayout.Space (2f);
	}

	static public void EndContents ()
	{
		GUILayout.Space (3f);
		GUILayout.EndVertical ();
		EditorGUILayout.EndHorizontal ();
		GUILayout.Space (3f);
		GUILayout.EndHorizontal ();
		GUILayout.Space (3f);
	}
}
