using UnityEngine;
using System.Collections;

namespace JWFramework.Private
{
	public class JWDataItem
	{
		private enum DataType
		{
			Int,
			Long,
			Float,
			Double,
			Bool,
			Refrence,
		}

		private object basedata;
		private DataType dataType;

		public JWDataItem (object basedata)
		{
			ChangeValue (basedata);
		}

		public void ChangeValue (object newData)
		{
			this.basedata = newData;
			if (newData is int) {
				dataType = DataType.Int;
			} else if (newData is long) {
				dataType = DataType.Long;
			} else if (newData is float) {
				dataType = DataType.Float;
			} else if (newData is double) {
				dataType = DataType.Double;
			} else if (newData is bool) {
				dataType = DataType.Bool;
			} else {
				dataType = DataType.Refrence;
			}
		}

		public object GetObject ()
		{
			return basedata;
		}

		public int GetInt (int defaultValue)
		{
			switch (dataType) {
			case DataType.Int:
				return (int)basedata;
			case DataType.Long:
				return (int)((long)basedata);
			case DataType.Float:
				return (int)((float)basedata);
			case DataType.Double:
				return (int)((double)basedata);
			default:
				return defaultValue;
			}
		}

		public long GetLong (long defaultValue)
		{
			switch (dataType) {
			case DataType.Int:
				return (long)((int)basedata);
			case DataType.Long:
				return (long)basedata;
			case DataType.Float:
				return (long)((float)basedata);
			case DataType.Double:
				return (long)((double)basedata);
			default:
				return defaultValue;
			}
		}

		public float GetFloat (float defaultValue)
		{
			switch (dataType) {
			case DataType.Int:
				return (float)((int)basedata);
			case DataType.Long:
				return (float)((long)basedata);
			case DataType.Float:
				return (float)basedata;
			case DataType.Double:
				return (float)((double)basedata);
			default:
				return defaultValue;
			}
		}

		public double GetDouble (double defaultValue)
		{
			switch (dataType) {
			case DataType.Int:
				return (double)((int)basedata);
			case DataType.Long:
				return (double)((long)basedata);
			case DataType.Float:
				return (double)((float)basedata);
			case DataType.Double:
				return (double)basedata;
			default:
				return defaultValue;
			}
		}

		public bool GetBool (bool defaultValue)
		{
			if (dataType == DataType.Bool) {
				return (bool)basedata;
			} else {
				return defaultValue;
			}
		}

		public string GetString (string defaultValue)
		{
			try {
				return basedata.ToString ();
			} catch {
			}
			return defaultValue;
		}

		public T GetData<T> (T defaultValue = null) where T : class
		{
			if (dataType == DataType.Refrence && basedata != null) {
				return basedata as T;
			} else {
				return defaultValue;
			}
		}
	}
}