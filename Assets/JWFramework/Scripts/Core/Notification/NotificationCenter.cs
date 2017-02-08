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

		private List<NotificationCenterItem> selectorPools;

		public NotificationCenter ()
		{
			selectorPools = new List<NotificationCenterItem> ();
		}

		public void AddObserver (object observer, Selector selector, string notificationName, object poster)
		{
			selectorPools.Add (new NotificationCenterItem (observer, selector, notificationName, poster));
		}

		public void PostNotification (Notification notification)
		{
			string notificationName = notification.name;
			foreach (var item in selectorPools) {
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

		/// <summary>
		/// Removes the observer.
		/// </summary>
		/// <param name="observer">Observer.</param>
		public void RemoveObserver (object observer)
		{
			RemoveObserver (observer, "", null);
		}

		/// <summary>
		/// Removes the observer. If poster or notificationName is null, it's mean ignore this param
		/// </summary>
		/// <param name="observer">Observer.</param>
		/// <param name="notificationName">Notification name.</param>
		/// <param name="poster">Poster.</param>
		public void RemoveObserver (object observer, string notificationName, object poster)
		{
			bool nameIsNil = string.IsNullOrEmpty (notificationName);
			bool posterIsNil = (poster == null);
			for (int i = selectorPools.Count - 1; i >= 0; i--) {
				var item = selectorPools [i];
				if (item.observer.Equals (observer)) {
					if (nameIsNil) {
						if (posterIsNil) {
							selectorPools.RemoveAt (i);
						} else {
							if (item.poster.Equals (poster)) {
								selectorPools.RemoveAt (i);
							}
						}
					} else {
						if (posterIsNil) {
							if (item.notificationName.Equals (notificationName)) {
								selectorPools.RemoveAt (i);
							}
						} else {
							if (item.notificationName.Equals (notificationName) && item.poster.Equals (poster)) {
								selectorPools.RemoveAt (i);
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