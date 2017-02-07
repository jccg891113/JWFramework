using UnityEngine;
using System.Collections;

namespace JWFramework.Private
{
	public class JWDataItem
	{
		private object basedata;

		public JWDataItem (object basedata)
		{
			this.basedata = basedata;
		}

		public void ChangeValue (object newData)
		{
			this.basedata = newData;
		}

		public object GetObject ()
		{
			return basedata;
		}

		public int GetInt (int defaultValue = -1)
		{
			try {
				return (int)basedata;
			} catch {
			}
			return defaultValue;
		}

		public long GetLong (long defaultValue = -1)
		{
			try {
				return (long)basedata;
			} catch {
			}
			return defaultValue;
		}

		public float GetFloat (float defaultValue = -1)
		{
			try {
				return (float)basedata;
			} catch {
			}
			return defaultValue;
		}

		public double GetDouble (double defaultValue = -1)
		{
			try {
				return (double)basedata;
			} catch {
			}
			return defaultValue;
		}

		public bool GetBool (bool defaultValue = false)
		{
			try {
				return (bool)basedata;
			} catch {
			}
			return defaultValue;
		}

		public string GetString (string defaultValue = "")
		{
			try {
				return (string)basedata;
			} catch {
			}
			return defaultValue;
		}
	}
}