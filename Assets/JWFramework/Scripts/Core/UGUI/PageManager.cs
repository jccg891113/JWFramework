using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using JWFramework.UGUI.Private;

namespace JWFramework.UGUI
{
	public class PageManager : MonoBehaviour
	{
		private static PageManager _ins;

		public static PageManager Ins {
			get {
				if (_ins == null) {
					throw new System.Exception ("Current scene not have PageManager");
				}
				return _ins;
			}
		}

		[Tooltip ("UI摄像机对应的unity摄像机")]
		public Camera uiCamera;
		
		[Tooltip ("page页面的根挂载点")]
		public Transform pageRoot;

		[SerializeField][Tooltip ("永不释放图集的图集名称")]
		private List<string> AlwaysInMemoryAtlasName;

		protected Private.PageManagerCore core;

		public string CurrPageName {
			get {
				return core.CurrPageName;
			}
		}

		void Awake ()
		{
			if (_ins == null) {
				_ins = this;
				core = new PageManagerCore (uiCamera, pageRoot, AlwaysInMemoryAtlasName);
			}
		}

		/// <summary>
		/// 打开页面
		/// </summary>
		/// <param name="pageName">页面在Resources/Pages路径下的名称.</param>
		/// <param name="param">页面参数.</param>
		public void Open (string pageName, JWData param = null)
		{
			core.Open (pageName, param);
		}

		public void CloseCurrPage ()
		{
			core.CloseCurrPage ();
		}

		public void Close (string pageName)
		{
			core.Close (pageName);
		}

		public void CloseCurrAndOpen (string pageName, JWData param = null)
		{
			core.CloseCurrAndOpen (pageName, param);
		}

		public void CloseGroupPages (List<string> pageNames)
		{
			core.CloseGroupPages (pageNames);
		}

		public void CloseAll ()
		{
			core.CloseAll ();
		}

		public T GetPage<T> ()where T:PageBase
		{
			return core.GetPage<T> ();
		}

		public bool AtlasCouldRelease (string atlasName)
		{
			return core.AtlasCouldRelease (atlasName);
		}

		#if UNITY_EDITOR
		void OnDrawGizmos ()
		{
			PageManagerCore._OnDrawGizmos ();
		}
		#endif
	}
}