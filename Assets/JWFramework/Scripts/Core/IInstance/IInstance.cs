using UnityEngine;
using System.Collections;

public abstract class IInstance<T> where T : new()
{
	protected static T _ins;

	public static T Instance {
		get {
			if (_ins == null) {
				_ins = new T ();
			}
			return _ins;
		}
	}
}
