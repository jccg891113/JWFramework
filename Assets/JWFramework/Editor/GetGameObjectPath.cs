using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace JWFramework.Editors
{
	public class GetGameObjectPath : Editor
	{
		[MenuItem ("Tools/Game Object Path/Print Select Transform Path")]
		public static void GenerateAnimatorAsset ()
		{
			var item = Selection.activeTransform;
			if (item == null) {
				Debug.LogError ("No select transform");
				return;
			}

			string path = "";
			while (item != null) {
				path = "/" + item.name + path;
				item = item.parent;
			}
			
			Debug.Log ("[Success] Path is \"" + path + "\"");
		}
	}
}