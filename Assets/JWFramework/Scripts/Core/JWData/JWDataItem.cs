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
				if (basedata is int) {
					return (int)basedata;
				} else if (basedata is long) {
					long tmp = (long)basedata;
					return (int)tmp;
				} else if (basedata is float) {
					float tmp = (float)basedata;
					return (int)tmp;
				} else if (basedata is double) {
					double tmp = (double)basedata;
					return (int)tmp;
				}
			} catch {
			}
			return defaultValue;
		}

		public long GetLong (long defaultValue = -1)
		{
			try {
				if (basedata is int) {
					int tmp = (int)basedata;
					return (long)tmp;
				} else if (basedata is long) {
					return (long)basedata;
				} else if (basedata is float) {
					float tmp = (float)basedata;
					return (long)tmp;
				} else if (basedata is double) {
					double tmp = (double)basedata;
					return (long)tmp;
				}
			} catch {
			}
			return defaultValue;
		}

		public float GetFloat (float defaultValue = -1)
		{
			try {
				if (basedata is int) {
					int tmp = (int)basedata;
					return (float)tmp;
				} else if (basedata is long) {
					long tmp = (long)basedata;
					return (float)tmp;
				} else if (basedata is float) {
					return (float)basedata;
				} else if (basedata is double) {
					double tmp = (double)basedata;
					return (float)tmp;
				}
			} catch {
			}
			return defaultValue;
		}

		public double GetDouble (double defaultValue = -1)
		{
			try {
				if (basedata is int) {
					int tmp = (int)basedata;
					return (double)tmp;
				} else if (basedata is long) {
					long tmp = (long)basedata;
					return (double)tmp;
				} else if (basedata is float) {
					float tmp = (float)basedata;
					return (double)tmp;
				} else if (basedata is double) {
					return (double)basedata;
				}
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
				return basedata.ToString ();
			} catch {
			}
			return defaultValue;
		}
	}
}