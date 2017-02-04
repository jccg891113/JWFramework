using UnityEngine;
using System.Collections;

namespace JWFramework
{
	public class Notification
	{
		public string name;
		public object anObject;
		public JWData userInfo;

		private Notification (string name, object anObject, JWData userInfo)
		{
			this.name = name;
			this.anObject = anObject;
			this.userInfo = userInfo;
		}

		public static Notification GetNotification (string name, object anObject)
		{
			return new Notification (name, anObject, null);
		}

		public static Notification GetNotification (string name, object anObject, JWData userInfo)
		{
			return new Notification (name, anObject, userInfo);
		}
	}
}