using UnityEngine;
using System.Collections;

namespace JWFramework.Tools
{
	public static class FuncTool
	{
		public static T AddMissingComponent<T> (this GameObject target) where T : Component
		{
			T comp = target.GetComponent<T> ();
			if (comp == null) {
				comp = target.AddComponent<T> ();
			}
			return comp;
		}
	}
}