using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CanEditMultipleObjects]
[CustomEditor (typeof(JW_EffectUVAnim))]
public class JW_EffectUVAnimEditor : Editor
{
	Color cColor;
	float insertTime = 0;

	public override void OnInspectorGUI ()
	{
		JW_EffectUVAnim animKit = target as JW_EffectUVAnim;
		GUILayout.Space (3f);

		serializedObject.Update ();

		animKit.delayTime = EditorGUILayout.FloatField ("Delay Time", animKit.delayTime, GUILayout.MinWidth (60f));
		animKit.offsetXSpeed = EditorGUILayout.FloatField ("UV Offset X Speed", animKit.offsetXSpeed, GUILayout.MinWidth (60f));
		animKit.offsetYSpeed = EditorGUILayout.FloatField ("UV Offset Y Speed", animKit.offsetYSpeed, GUILayout.MinWidth (60f));
		
		animKit.needColor = EditorGUILayout.Toggle ("Need Color Anim", animKit.needColor, GUILayout.MinWidth (60f));
		
		if (animKit.needColor) {
			animKit.tintColor = EditorGUILayout.Toggle ("Addtive", animKit.tintColor, GUILayout.MinWidth (60f));
			animKit.colorR = EditorGUILayout.CurveField ("R", animKit.colorR, GUILayout.MinWidth (60f));
			animKit.colorG = EditorGUILayout.CurveField ("G", animKit.colorG, GUILayout.MinWidth (60f));
			animKit.colorB = EditorGUILayout.CurveField ("B", animKit.colorB, GUILayout.MinWidth (60f));
			animKit.colorA = EditorGUILayout.CurveField ("A", animKit.colorA, GUILayout.MinWidth (60f));
			
			cColor = EditorGUILayout.ColorField ("New Color", cColor);
			insertTime = EditorGUILayout.FloatField ("Insert Color Time", insertTime);
			EditorGUILayout.BeginHorizontal ();
			if (GUILayout.Button ("Insert Color")) {
				try {
					animKit.colorR.AddKey (insertTime, cColor.r);
					animKit.colorG.AddKey (insertTime, cColor.g);
					animKit.colorB.AddKey (insertTime, cColor.b);
					animKit.colorA.AddKey (insertTime, cColor.a);
				} catch {

				}
			}
			if (GUILayout.Button ("Delete Color")) {
				try {
					for (int i = 0, imax = animKit.colorR.length; i < imax; i++) {
						if (animKit.colorR.keys [i].time == insertTime) {
							animKit.colorR.RemoveKey (i);
							break;
						}
					}
					for (int i = 0, imax = animKit.colorG.length; i < imax; i++) {
						if (animKit.colorG.keys [i].time == insertTime) {
							animKit.colorG.RemoveKey (i);
							break;
						}
					}
					for (int i = 0, imax = animKit.colorB.length; i < imax; i++) {
						if (animKit.colorB.keys [i].time == insertTime) {
							animKit.colorB.RemoveKey (i);
							break;
						}
					}
					for (int i = 0, imax = animKit.colorA.length; i < imax; i++) {
						if (animKit.colorA.keys [i].time == insertTime) {
							animKit.colorA.RemoveKey (i);
							break;
						}
					}
				} catch {

				}
			}
			GUILayout.EndHorizontal ();
		}
		
		serializedObject.ApplyModifiedProperties ();
	}
}
