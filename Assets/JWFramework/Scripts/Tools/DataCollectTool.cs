using UnityEngine;
using UnityEngine.Analytics;
using System.Collections;
using System.Collections.Generic;

namespace JWFramework.Tools
{
	public class DataCollectTool
	{
		public static void CustomEvent (string customEventName, IDictionary<string, object> eventData)
		{
			#if UNITY_5_4_OR_NEWER
			UnityEngine.Analytics.Analytics.CustomEvent (customEventName, eventData);
			#endif
		}
	}
}