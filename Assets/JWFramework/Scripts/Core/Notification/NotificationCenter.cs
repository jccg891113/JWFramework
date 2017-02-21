using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace JWFramework
{
	public delegate void Selector (Notification notification);
	
	/// <summary>
	/// An NotificationCenter object (or simply, notification center) provides a mechanism 
	/// for broadcasting information within a program. An NotificationCenter object is essentially 
	/// a notification dispatch table.
	/// </summary>
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

		private Dictionary<object, List<NotificationCenterItem>> receiversDispatchTable;

		public NotificationCenter ()
		{
			receiversDispatchTable = new Dictionary<object, List<NotificationCenterItem>> ();
		}

		/// <summary>
		/// Adds an entry to the receiver’s dispatch table with an observer, a notification selector and optional criteria: notification name and sender.
		/// </summary>
		/// <param name="observer">Object registering as an observer. This value must not be null.</param>
		/// <param name="selector">Selector that specifies the message the receiver sends observer to notify it of the notification posting. 
		/// The method specified by selector must have one and only one argument (an instance of Notification).</param>
		/// <param name="notificationName">The name of the notification for which to register the observer; that is, only notifications with this name are delivered to the observer.
		/// If you pass null, the notification center doesn’t use a notification’s name to decide whether to deliver it to the observer.</param>
		/// <param name="poster">The object whose notifications the observer wants to receive; that is, only notifications sent by this sender are delivered to the observer.
		/// If you pass null, the notification center doesn’t use a notification’s sender to decide whether to deliver it to the observer.</param>
		public void AddObserver (object observer, Selector selector, string notificationName, object sender)
		{
			if (observer == null) {
				throw new System.Exception ("The observer must not be null");
			}
			if (string.IsNullOrEmpty (notificationName)) {
				throw new System.Exception ("The notification name must not be null or empty");
			}
			if (!receiversDispatchTable.ContainsKey (observer) || receiversDispatchTable [observer] == null) {
				receiversDispatchTable [observer] = new List<NotificationCenterItem> ();
			}
			receiversDispatchTable [observer].Add (new NotificationCenterItem (selector, notificationName, sender));
		}

		/// <summary>
		/// Posts a given notification to the receiver.
		/// </summary>
		/// <param name="notification">The notification to post. This value must not be null.</param>
		public void PostNotification (Notification notification)
		{
			if (notification == null) {
				throw new System.Exception ("The notification must not be null");
			}
			string notificationName = notification.name;
			foreach (var receivers in receiversDispatchTable) {
				foreach (var item in receivers.Value) {
					if (item.notificationName.Equals (notificationName)) {
						item.selector (notification);
					}
				}
			}
		}

		/// <summary>
		/// Creates a notification with a given name and sender and posts it to the receiver.
		/// </summary>
		/// <param name="notificationName">The name of the notification.</param>
		/// <param name="poster">The object posting the notification.</param>
		public void PostNotification (string notificationName, object poster)
		{
			PostNotification (notificationName, poster, null);
		}

		/// <summary>
		/// Creates a notification with a given name, sender, and information and posts it to the receiver.
		/// </summary>
		/// <param name="notificationName">The name of the notification.</param>
		/// <param name="poster">The object posting the notification.</param>
		/// <param name="userInfo">Information about the the notification. May be null.</param>
		public void PostNotification (string notificationName, object poster, JWData userInfo)
		{
			Notification notification = Notification.GetNotification (notificationName, poster, userInfo);
			PostNotification (notification);
		}

		/// <summary>
		/// Removes all the entries specifying a given observer from the receiver’s dispatch table.
		/// </summary>
		/// <param name="observer">The observer to remove. Must not be null.</param>
		public void RemoveObserver (object observer)
		{
			RemoveObserver (observer, "", null);
		}

		/// <summary>
		/// Removes matching entries from the receiver’s dispatch table.
		/// </summary>
		/// <param name="observer">Observer to remove from the dispatch table. Specify an observer to remove only entries for this observer. 
		/// Must not be null, or message will have no effect.</param>
		/// <param name="notificationName">Name of the notification to remove from dispatch table. Specify a notification name to remove only 
		/// entries that specify this notification name. When null or empty, the receiver does not use notification names as criteria for removal.</param>
		/// <param name="poster">Sender to remove from the dispatch table. Specify a notification sender to remove only entries that specify this sender. 
		/// When null, the receiver does not use notification senders as criteria for removal.</param>
		public void RemoveObserver (object observer, string notificationName, object poster)
		{
			if (observer == null) {
				throw new System.Exception ("The observer must not be null");
			}
			if (!receiversDispatchTable.ContainsKey (observer)) {
				return;
			}
			bool nameIsNil = string.IsNullOrEmpty (notificationName);
			bool posterIsNil = (poster == null);
			if (nameIsNil && posterIsNil) {
				receiversDispatchTable.Remove (observer);
			} else {
				for (int i = receiversDispatchTable [observer].Count - 1; i >= 0; i--) {
					var item = receiversDispatchTable [observer] [i];
					if (nameIsNil && !posterIsNil) {
						if (item.poster.Equals (poster)) {
							receiversDispatchTable [observer].RemoveAt (i);
						}
					} else if (!nameIsNil && posterIsNil) {
						if (item.notificationName.Equals (notificationName)) {
							receiversDispatchTable [observer].RemoveAt (i);
						}
					} else if (!nameIsNil && !posterIsNil) {
						if (item.notificationName.Equals (notificationName) && item.poster.Equals (poster)) {
							receiversDispatchTable [observer].RemoveAt (i);
						}
					}
				}
			}
		}

		private class NotificationCenterItem
		{
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

			public NotificationCenterItem (Selector selector, string notificationName, object poster)
			{
				this.selector = selector;
				this.notificationName = notificationName;
				this.poster = poster;
			}
		}
	}
}