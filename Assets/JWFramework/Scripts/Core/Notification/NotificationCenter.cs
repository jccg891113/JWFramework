using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace JWFramework
{
	public delegate void Selector (Notification notification);
	
	public class NotificationCenter
	{
		private static NotificationCenter _ins = null;

		public static NotificationCenter Default {
			get {
				if (_ins == null) {
					_ins = new NotificationCenter ();
				}
				return _ins;
			}
		}

		private List<NotificationCenterItem> allSelectors;

		public NotificationCenter ()
		{
			allSelectors = new List<NotificationCenterItem> ();
		}

		public void AddObserver (object observer, Selector selector, string notificationName, object poster)
		{
			allSelectors.Add (new NotificationCenterItem (observer, selector, notificationName, poster));
		}

		public void PostNotification (Notification notification)
		{
			string notificationName = notification.name;
			foreach (var item in allSelectors) {
				if (item.notificationName.Equals (notificationName)) {
					item.selector (notification);
				}
			}
		}

		public void PostNotification (string notificationName, object poster)
		{
			Notification notification = Notification.GetNotification (notificationName, poster);
			PostNotification (notification);
		}

		public void PostNotification (string notificationName, object poster, JWData userInfo)
		{
			Notification notification = Notification.GetNotification (notificationName, poster, userInfo);
			PostNotification (notification);
		}

		public void RemoveObserver (object observer)
		{
			RemoveObserver (observer, "", null);
		}

		public void RemoveObserver (object observer, string notificationName, object poster)
		{
			bool nameIsNil = string.IsNullOrEmpty (notificationName);
			bool posterIsNil = (poster == null);
			for (int i = allSelectors.Count - 1; i >= 0; i--) {
				var item = allSelectors [i];
				if (item.observer.Equals (observer)) {
					if (nameIsNil) {
						if (posterIsNil) {
							allSelectors.RemoveAt (i);
						} else {
							if (item.poster.Equals (poster)) {
								allSelectors.RemoveAt (i);
							}
						}
					} else {
						if (posterIsNil) {
							if (item.notificationName.Equals (notificationName)) {
								allSelectors.RemoveAt (i);
							}
						} else {
							if (item.notificationName.Equals (notificationName) && item.poster.Equals (poster)) {
								allSelectors.RemoveAt (i);
							}
						}
					}
				}
			}
		}

		private class NotificationCenterItem
		{
			/// <summary>
			/// Key
			/// </summary>
			public object observer;
			/// <summary>
			/// Key Condition
			/// </summary>
			public string notificationName;
			/// <summary>
			/// Key Condition
			/// </summary>
			public object poster;
			/// <summary>
			/// Core
			/// </summary>
			public Selector selector;

			public NotificationCenterItem (object observer, Selector selector, string notificationName, object poster)
			{
				this.observer = observer;
				this.selector = selector;
				this.notificationName = notificationName;
				this.poster = poster;
			}
		}
	}
}