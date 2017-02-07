using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using JWFramework.Private;

namespace JWFramework
{
	public class JWData
	{
		private Dictionary<string, JWDataItem> baseData;

		public JWData (params object[] paramKeyValue)
		{
			baseData = new Dictionary<string, JWDataItem> ();
			if ((paramKeyValue.Length % 2) != 0) {
				throw new System.Exception ("JWData initiation error: param number asymmetry");
			}
			for (int i = 0; i < paramKeyValue.Length; i += 2) {
				baseData [paramKeyValue [i].ToString ()] = new JWDataItem (paramKeyValue [i + 1]);
			}
		}

		public void Insert (string key, object value)
		{
			baseData [key] = new JWDataItem (value);
		}

		public void Change (string key, object value)
		{
			if (Contains (key)) {
				baseData [key].ChangeValue (value);
			} else {
				Insert (key, value);
			}
		}

		public bool Contains (string key)
		{
			return baseData.ContainsKey (key);
		}

		public JWDataItem GetDataItem (string key, object defaultValue = null)
		{
			if (baseData.ContainsKey (key)) {
				return baseData [key];
			} else {
				JWDebug.LogError ("[ERROR] Unable to obtain \'" + key + "\' from JWData");
				return new JWDataItem (defaultValue);
			}
		}

		public object this [string key] {
			get {
				return GetObject (key, null);
			}
			set {
				Change (key, value);
			}
		}

		public object GetObject (string key, object defaultValue = null)
		{
			return GetDataItem (key, defaultValue).GetObject ();
		}

		public string GetString (string key, string defaultValue = "")
		{
			return GetDataItem (key, defaultValue).GetString (defaultValue);
		}

		public int GetInt (string key, int defaultValue = -1)
		{
			return GetDataItem (key, defaultValue).GetInt (defaultValue);
		}

		public long GetLong (string key, long defaultValue = -1)
		{
			return GetDataItem (key, defaultValue).GetLong (defaultValue);
		}

		public float GetFloat (string key, float defaultValue = -1)
		{
			return GetDataItem (key, defaultValue).GetFloat (defaultValue);
		}

		public double GetDouble (string key, double defaultValue = -1)
		{
			return GetDataItem (key, defaultValue).GetDouble (defaultValue);
		}

		public bool GetBool (string key, bool defaultValue = false)
		{
			return GetDataItem (key, defaultValue).GetBool (defaultValue);
		}
	}
}