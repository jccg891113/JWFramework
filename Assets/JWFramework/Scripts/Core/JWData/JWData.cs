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
			for (int i = 0, imax = paramKeyValue.Length; i < imax; i += 2) {
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

		private JWDataItem GetDataItem (string key)
		{
			if (Contains (key)) {
				return baseData [key];
			} else {
				JWDebug.LogError ("[ERROR] Unable to obtain \'" + key + "\' from JWData");
				return null;
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

		public int GetInt (string key, int defaultValue = -1)
		{
			JWDataItem dataItem = GetDataItem (key);
			if (dataItem != null) {
				return dataItem.GetInt (defaultValue);
			} else {
				return defaultValue;
			}
		}

		public long GetLong (string key, long defaultValue = -1)
		{
			JWDataItem dataItem = GetDataItem (key);
			if (dataItem != null) {
				return dataItem.GetLong (defaultValue);
			} else {
				return defaultValue;
			}
		}

		public float GetFloat (string key, float defaultValue = -1)
		{
			JWDataItem dataItem = GetDataItem (key);
			if (dataItem != null) {
				return dataItem.GetFloat (defaultValue);
			} else {
				return defaultValue;
			}
		}

		public double GetDouble (string key, double defaultValue = -1)
		{
			JWDataItem dataItem = GetDataItem (key);
			if (dataItem != null) {
				return dataItem.GetDouble (defaultValue);
			} else {
				return defaultValue;
			}
		}

		public bool GetBool (string key, bool defaultValue = false)
		{
			JWDataItem dataItem = GetDataItem (key);
			if (dataItem != null) {
				return dataItem.GetBool (defaultValue);
			} else {
				return defaultValue;
			}
		}

		public string GetString (string key, string defaultValue = "")
		{
			JWDataItem dataItem = GetDataItem (key);
			if (dataItem != null) {
				return dataItem.GetString (defaultValue);
			} else {
				return defaultValue;
			}
		}

		public object GetObject (string key, object defaultValue = null)
		{
			JWDataItem dataItem = GetDataItem (key);
			if (dataItem != null) {
				return dataItem.GetObject ();
			} else {
				return defaultValue;
			}
		}

		public T GetData<T> (string key, T defaultValue = null) where T : class
		{
			JWDataItem dataItem = GetDataItem (key);
			if (dataItem != null) {
				return dataItem.GetData<T> (defaultValue);
			} else {
				return defaultValue;
			}
		}
	}
}