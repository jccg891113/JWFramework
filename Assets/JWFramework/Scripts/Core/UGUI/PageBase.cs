using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using JWFramework.UGUI.Private;

namespace JWFramework.UGUI
{
	public enum PageType
	{
		FULL_SCREEN,
		COVER,
	}

	public enum PageSortingLayer
	{
		Default = 0,
		Cus1 = 1,
		Cus2 = 2,
		Cus3 = 3,
		Cus4 = 4,
	}

	public enum HideType
	{
		OutScreen,
		Disable,
		DisableAndRelease,
	}

	public enum CloseType
	{
		Disable,
		Destroy,
	}

	public enum PageState
	{
		CLOSE,
		OPEN,
		HIDE,
		BACKGROUND,
	}

	public class PageBase : MonoBehaviour
	{
		public PageType pageType;
		public PageSortingLayer sortingLayerType = PageSortingLayer.Default;
		public HideType hideType = HideType.DisableAndRelease;
		public CloseType closeType = CloseType.Destroy;
		public PageCanvasData canvasData;
		public PageTextureData textureData;

		public string pageName{ private set; get; }

		public PageState lastState{ private set; get; }

		public PageState currState{ private set; get; }

		private JWData param;

		private List<JWData> historyParams = new List<JWData> ();

		public void SetPageName (string pageName)
		{
			this.pageName = pageName;
		}

		public void InitFirstOpen ()
		{
			lastState = PageState.CLOSE;
			currState = PageState.CLOSE;
			canvasData = new PageCanvasData (GetComponentsInChildren<Canvas> ());
			textureData = new PageTextureData (GetComponentsInChildren<Image> ());
			this._InitFirstOpen ();
		}

		protected virtual void _InitFirstOpen ()
		{
		}

		public void SaveParamToHistory ()
		{
			historyParams.Add (param);
		}

		public void ResetParamFromHistory ()
		{
			if (historyParams.Count > 0) {
				param = historyParams [historyParams.Count - 1];
				historyParams.RemoveAt (historyParams.Count - 1);
			} else {
				throw new System.Exception ("No history param on " + gameObject.name);
			}
		}

		#region Operation Current Page Param Data

		/// <summary>
		/// 保存当前页面显示参数，重新打开页面会根据参数刷新
		/// </summary>
		/// <param name="param">Parameter.</param>
		protected void ResaveCurrParam (JWData param)
		{
			if (param != null) {
				this.param = param;
			} else {
				throw new System.Exception ("ResaveParam can not save null!");
			}
		}

		protected void ChangeParam (string key, object value)
		{
			param.Change (key, value);
		}

		protected bool ParamContains (string key)
		{
			return param.Contains (key);
		}

		protected string GetParamString (string key, string defaultValue = "")
		{
			return param.GetString (key, defaultValue);
		}

		protected int GetParamInt (string key, int defaultValue = -1)
		{
			return param.GetInt (key, defaultValue);
		}

		protected float GetParamFloat (string key, float defaultValue = -1)
		{
			return param.GetFloat (key, defaultValue);
		}

		protected double GetParamDouble (string key, double defaultValue = -1)
		{
			return param.GetDouble (key, defaultValue);
		}

		protected long GetParamLong (string key, long defaultValue = -1)
		{
			return param.GetLong (key, defaultValue);
		}

		protected bool GetParamBool (string key, bool defaultValue = false)
		{
			return param.GetBool (key, defaultValue);
		}

		protected object GetParamObject (string key, object defaultValue = null)
		{
			return param.GetObject (key, defaultValue);
		}

		#endregion

		public void SetOpenParam (JWData param)
		{
			this.param = (param != null) ? param : new JWData ();
		}

		public void TransPageState (PageState newState)
		{
			lastState = currState;
			currState = newState;
		}

		public virtual void PageOpen ()
		{
		}

		public virtual void PageHide ()
		{
		}

		public virtual void PageBack ()
		{
		}

		public virtual void PageClose ()
		{
		}

		public void Close ()
		{
			PageManager.Ins.Close ();
		}
	}
}