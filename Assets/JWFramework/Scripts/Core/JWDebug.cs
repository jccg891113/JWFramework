using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace JWFramework
{
	public class JWDebug
	{
		public enum LogType
		{
			normal = 0x01,
			net_socket = 0x02,
			net_http = 0x04,
		}

		public static int LogBlock = 0xFE;

		public static void Log (object message, LogType logType = LogType.normal)
		{
			#if UNITY_EDITOR
			Debug.Log ("[EDITOR] " + message.ToString ());
			#else
			if (LogBlock & (int)logType > 0) {
				Debug.Log ("[EDITOR] " + message.ToString ());
			}
			#endif
		}

		public static void LogWarning (object message, LogType logType = LogType.normal)
		{
			#if UNITY_EDITOR
			Debug.LogWarning ("[EDITOR] " + message.ToString ());
			#else
			if (LogBlock & (int)logType > 0) {
				Debug.LogWarning ("[EDITOR] " + message.ToString ());
			}
			#endif
		}

		public static void LogError (object message, LogType logType = LogType.normal)
		{
			#if UNITY_EDITOR
			//string consolePath = @"E:\WorkSpace\ZuoYouHuYu\L1Tech\CheckServer\console1.txt";
			//FileStream fs = new FileStream(consolePath, FileMode.Append, FileAccess.Write);
			//StreamWriter sw = new StreamWriter(fs);
			//sw.WriteLine(message.ToString());
			//sw.Flush();
			////关闭流
			//sw.Close();
			//fs.Close();
			Debug.LogError ("[EDITOR] " + message.ToString ());
			#else
			if (LogBlock & (int)logType > 0) {
				Debug.LogError ("[EDITOR] " + message.ToString ());
			}
			#endif
		}

		public static void LogFormat (string format, params object[] args)
		{
			#if UNITY_EDITOR
			Debug.Log ("[EDITOR] " + string.Format (format, args));
			#endif
		}

		public static void LogWarningFormat (string format, params object[] args)
		{
			#if UNITY_EDITOR
			Debug.LogWarning ("[EDITOR] " + string.Format (format, args));
			#endif
		}

		public static void LogErrorFormat (string format, params object[] args)
		{
			#if UNITY_EDITOR
			Debug.LogError ("[EDITOR] " + string.Format (format, args));
			#endif
		}

		#if UNITY_EDITOR
		private static Dictionary<string, System.Diagnostics.Stopwatch> timeGroup;
		#endif

		public static void LogTimeStart (string key)
		{
			#if UNITY_EDITOR
			if (timeGroup == null) {
				timeGroup = new Dictionary<string, System.Diagnostics.Stopwatch> ();
			}
			timeGroup [key] = new System.Diagnostics.Stopwatch ();
			timeGroup [key].Start ();
			#endif
		}

		public static void LogTimeCost (string key)
		{
			#if UNITY_EDITOR
			if (timeGroup.ContainsKey (key)) {
				timeGroup [key].Stop ();
				Log (string.Format ("[Time Log] {0} : {1} ms", key, timeGroup [key].ElapsedMilliseconds));
				timeGroup.Remove (key);
			}
			#endif
		}
	}
}