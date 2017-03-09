using UnityEngine;
using System.Collections;

namespace JWFramework
{
	public class Notification
	{
		public string name;
		public object poster;
		public JWData userInfo;

		private Notification (string name, object poster, JWData userInfo)
		{
			this.name = name;
			this.poster = poster;
			this.userInfo = userInfo;
		}

		public static Notification GetNotification (string name, object poster)
		{
			return new Notification (name, poster, null);
		}

		public static Notification GetNotification (string name, object poster, JWData userInfo)
		{
			return new Notification (name, poster, userInfo);
		}
	}
}