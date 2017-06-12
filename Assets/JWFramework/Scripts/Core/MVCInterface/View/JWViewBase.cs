using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace JWFramework.MVC
{
	public abstract class JWViewBase : MonoBehaviour
	{
		public void Awake ()
		{
		}

		protected abstract string[] GetNotificationName ();
	}
}