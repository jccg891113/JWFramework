using UnityEngine;
using System.Collections;

namespace JWFramework
{
	public class MonoInstanceGenerater<T> where T : MonoBehaviour
	{
		public static T GenerateIns (Vector3 defaultPos, string cusName = "")
		{
			GameObject go = new GameObject ("JWFramework_" + typeof(T) + cusName);
			go.transform.localScale = Vector3.one;
			go.transform.position = defaultPos;
			var _instance = go.AddComponent<T> ();
			Object.DontDestroyOnLoad (go);
			return _instance;
		}
	}
}

