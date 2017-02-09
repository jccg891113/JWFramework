using UnityEngine;
using System.Collections;

public class MonoInstance<T> : MonoBehaviour where T : MonoInstance<T>
{
	protected static T _ins;

	public static T Instance {
		get {
			if (_ins == null) {
				GameObject go = new GameObject ("_" + typeof(T));
				go.transform.position = Vector3.zero;
				_ins = go.AddComponent<T> ();
				_ins.Init ();
				DontDestroyOnLoad (go);
			}
			return _ins;
		}
	}

	protected virtual void Init ()
	{
	}
}
