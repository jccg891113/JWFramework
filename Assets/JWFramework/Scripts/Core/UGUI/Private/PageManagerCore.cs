using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace JWFramework.UGUI.Private
{
	public class PageManagerCore
	{
		public Camera uiCamera;

		private Transform pageRoot;

		private Private.PageGroups pageHistory;

		/// <summary>
		/// 页面堆，用于保存已打开过的常驻内存页面
		/// </summary>
		private Private.PageHeap pageHeap;

		/// <summary>
		/// 当前已打开页面
		/// </summary>
		private PageBase currPage { get { return pageHistory.LastPage; } }

		private List<string> AlwaysInMemoryAtlasName;

		public PageManagerCore (Camera uiCamera, Transform pageRoot, List<string> AlwaysInMemoryAtlasName)
		{
			this.uiCamera = uiCamera;
			this.pageRoot = pageRoot;
			this.AlwaysInMemoryAtlasName = new List<string> (AlwaysInMemoryAtlasName);
			pageHistory = new UGUI.Private.PageGroups ();
			pageHeap = new UGUI.Private.PageHeap ();
		}

		public string CurrPageName {
			get {
				if (currPage != null) {
					return currPage.pageName;
				} else {
					return "";
				}
			}
		}

		#region Open

		/// <summary>
		/// 打开页面
		/// </summary>
		/// <param name="pageName">页面在Resources/Pages路径下的名称.</param>
		/// <param name="param">页面参数.</param>
		public void Open (string pageName, JWData param = null)
		{
			PageBase newPage = _GetPage (pageName);
			_OperationLastPages (newPage.pageType);
			_OpeParamBeforeOpenPage (newPage);
			_PageShow (newPage, param);
			_SortCurrGroup ();
			PageManagerEvent.UGUI_PageManager_QueueChange (pageHistory);
		}

		#endregion

		#region Open - GetPage

		private PageBase _GetPage (string pageName)
		{
			PageBase page = __GetPageFromMemory (pageName);
			return (page != null) ? page : __GetPageFromResource (pageName);
		}

		private PageBase __GetPageFromMemory (string pageName)
		{
			PageBase page = pageHistory.GetPage (pageName);
			return (page != null) ? page : pageHeap.GetPage (pageName);
		}

		/// <summary>
		/// 由Resources中加载页面
		/// </summary>
		/// <returns>结果页面</returns>
		/// <param name="pageName">页面名称</param>
		private PageBase __GetPageFromResource (string pageName)
		{
			PageBase page = null;
			Object obj = Resources.Load ("Pages/" + pageName);
			if (obj != null) {
				GameObject go = MonoBehaviour.Instantiate (obj) as GameObject;
				go.name = pageName;
				___InitPageTransform (go);
				___InitPageRectTransform (go);
				page = go.GetComponent<PageBase> ();
				if (page != null) {
					page.SetPageName (pageName);
				} else {
					throw new System.Exception ("Unable load page \'" + pageName + "\'\nAt JWFramework.UGUI.Private.PageManagerCore.GetPageFromResource (string pageName)");
				}
			}
			return page;
		}

		private void ___InitPageTransform (GameObject goPage)
		{
			goPage.transform.SetParent (pageRoot);
			goPage.transform.localPosition = Vector3.zero;
			goPage.transform.localScale = Vector3.one;
		}

		private void ___InitPageRectTransform (GameObject goPage)
		{
			RectTransform rt = goPage.GetComponent<RectTransform> ();
			rt.anchorMin = Vector2.zero;
			rt.anchorMax = Vector2.one;
			rt.offsetMin = Vector2.zero;
			rt.offsetMax = Vector2.zero;
		}

		#endregion

		#region Open - Main - Operation Last Pages

		private void _OperationLastPages (PageType newPageType)
		{
			if (newPageType == PageType.FULL_SCREEN) {
				__HideLastGroup ();
			} else {
				__LetCurrPageBack ();
			}
		}

		private void __HideLastGroup ()
		{
			if (pageHistory.LastGroup != null) {
				foreach (var pageName in pageHistory.LastGroup.pageQueue) {
					var page = pageHistory.GetPage (pageName);
					page.PriPageHide ();
				}
			}
		}

		private void __LetCurrPageBack ()
		{
			if (currPage != null) {
				currPage.PriPageBack ();
			}
		}

		#endregion

		#region Open - Main - Init New Page

		private void _OpeParamBeforeOpenPage (PageBase newPage)
		{
			bool firstOpen = __IsPageFirstOpen (newPage.pageName);
			if (firstOpen) {
				__InitPage (newPage);
			} else {
				__SaveCurrParam (newPage);
			}
		}

		private bool __IsPageFirstOpen (string pageName)
		{
			return !pageHistory.PageInHistory (pageName);
		}

		private void __InitPage (PageBase newPage)
		{
			newPage.InitFirstOpen ();
		}

		private void __SaveCurrParam (PageBase newPage)
		{
			newPage.SaveParamToHistory ();
		}

		#endregion

		#region Open - Main - Show New Page

		private void _PageShow (PageBase page, JWData param)
		{
			__PutNewPageEnqueue (page);
			page.PriPageOpen (param);
			__ReleaseTextureAfterOpen ();
		}

		private void __PutNewPageEnqueue (PageBase page)
		{
			pageHistory.PageEnqueue (page);
		}

		private void __ReleaseTextureAfterOpen ()
		{
			List<Texture> allGroupImportantTextures = pageHistory.AllGroupImportantTextures;
			List<Texture> otherGroupTextures = pageHistory.OtherGroupReleaseTextures;
			ReleaseResource (allGroupImportantTextures, otherGroupTextures);
		}

		#endregion

		#region Close

		public void CloseCurrPage ()
		{
			PageBase closePage = this.currPage;
			if (closePage != null) {
				_LetPageDequeue ();
				_CloseMain (closePage);
				_ReshowOtherPages (closePage);
			}
			_SortCurrGroup ();
			PageManagerEvent.UGUI_PageManager_QueueChange (pageHistory);
		}

		public void Close (string pageName)
		{
			PageBase closePage = this.currPage;
			if (closePage != null && closePage.pageName == pageName) {
				_LetPageDequeue ();
				_CloseMain (closePage);
				_ReshowOtherPages (closePage);
			}
			_SortCurrGroup ();
			PageManagerEvent.UGUI_PageManager_QueueChange (pageHistory);
		}

		#endregion

		#region Close - Page Dequeue

		private void _LetPageDequeue ()
		{
			pageHistory.LastPageDequeue ();
		}

		#endregion

		#region Close - Main

		private void _CloseMain (PageBase closePage)
		{
			bool pageWillHide = __ClosePageNeedHiden (closePage);
			if (pageWillHide) {
				__HidePage (closePage);
			} else {
				__ClosePage (closePage);
			}
			__ReleaseResourceAfterClose (closePage, pageWillHide);
		}

		private bool __ClosePageNeedHiden (PageBase closePage)
		{
			return pageHistory.PageInHistory (closePage.pageName);
		}

		#endregion

		#region Close - Main - Hide Page

		private void __HidePage (PageBase closePage)
		{
			closePage.ResetParamFromHistory ();
			closePage.PriPageHide ();
		}

		#endregion

		#region Close - Main - Close Page

		private void __ClosePage (PageBase closePage)
		{
			closePage.TransPageState (PageState.CLOSE);
			closePage.PageClose ();
			___ClosePageMain (closePage);
			___PutClosePageInHeap (closePage);
		}

		private void ___ClosePageMain (PageBase closePage)
		{
			if (closePage.closeType == CloseType.Disable) {
				closePage.gameObject.SetActive (false);
			} else {
				MonoBehaviour.Destroy (closePage.gameObject);
			}
		}

		private void ___PutClosePageInHeap (PageBase closePage)
		{
			if (closePage.closeType == CloseType.Disable && !pageHeap.Contains (closePage.name)) {
				pageHeap.Add (closePage);
			}
		}

		#endregion

		#region Close - Main - Release Resource

		private void __ReleaseResourceAfterClose (PageBase closePage, bool pageWillHide)
		{
			List<Texture> allGroupImportantTextures = pageHistory.AllGroupImportantTextures;
			List<Texture> otherGroupTextures = pageHistory.OtherGroupReleaseTextures;
			if (!pageWillHide || closePage.hideType == HideType.DisableAndRelease) {
				for (int i = 0, imax = closePage.textureData.referencedTextures.Count; i < imax; i++) {
					if (!otherGroupTextures.Contains (closePage.textureData.referencedTextures [i])) {
						otherGroupTextures.Add (closePage.textureData.referencedTextures [i]);
					}
				}
			}
			ReleaseResource (allGroupImportantTextures, otherGroupTextures);
		}

		#endregion

		#region Close - Reshow Other Pages

		private void _ReshowOtherPages (PageBase closePage)
		{
			if (closePage.pageType == PageType.FULL_SCREEN) {
				__ReshowLastGroup ();
			} else {
				if (currPage != null) {
					currPage.PriPageReopen ();
				}
			}
		}

		private void __ReshowLastGroup ()
		{
			if (pageHistory.LastGroup != null) {
				foreach (var p in pageHistory.LastGroup.pageQueue) {
					var page = pageHistory.GetPage (p);
					if (page != currPage) {
						page.PriPageBack ();
					} else {
						page.PriPageReopen ();
					}
				}
			}
		}

		#endregion

		#region Close And Open

		public void CloseCurrAndOpen (string pageName, JWData param = null)
		{
			// close
			PageBase closePage = this.currPage;
			if (closePage != null) {
				_LetPageDequeue ();
				_CloseMain (closePage);
			}
			// open
			PageBase currPage = _GetPage (pageName);
			_CloseAndOpenOperationLastPages (closePage, currPage);
			_OpeParamBeforeOpenPage (currPage);
			_PageShow (currPage, param);
			// sort
			_SortCurrGroup ();
			PageManagerEvent.UGUI_PageManager_QueueChange (pageHistory);
		}

		private void _CloseAndOpenOperationLastPages (PageBase closePage, PageBase newPage)
		{
			if (closePage != null) {
				if (newPage.pageType == PageType.FULL_SCREEN) {
					__HideLastGroup ();
				} else {
					__BackLastGroup ();
				}
			}
		}

		private void __BackLastGroup ()
		{
			if (pageHistory.LastGroup != null) {
				foreach (var _pageName in pageHistory.LastGroup.pageQueue) {
					var page = pageHistory.GetPage (_pageName);
					page.PriPageBack ();
				}
			}
		}

		#endregion

		private void ReleaseResource (List<Texture> importantTextures, List<Texture> couldReleaseTextures)
		{
			for (int i = 0, imax = couldReleaseTextures.Count; i < imax; i++) {
				Texture texRef = couldReleaseTextures [i];
				if (texRef != null) {
					bool important = importantTextures.Contains (texRef);
					if (!important) {
						JWDebug.Log ("[Texture Release] " + texRef.name);
						Resources.UnloadAsset (texRef);
					}
				}
			}
		}

		#region Close All

		public void CloseAll ()
		{
			_CloseAllLastPage ();
			_CloseAllReleaseResource ();
			_CloseAllMain ();
			PageManagerEvent.UGUI_PageManager_QueueChange (pageHistory);
		}

		private void _CloseAllLastPage ()
		{
			PageBase closePage = this.currPage;
			if (closePage != null) {
				closePage.PriPageClose ();
			}
		}

		private void _CloseAllReleaseResource ()
		{
			List<Texture> releaseTextures = pageHistory.AllGroupReleaseTextures;
			ReleaseResource (new List<Texture> (), releaseTextures);
		}

		private void _CloseAllMain ()
		{
			foreach (var page in pageHistory.AllPages) {
				page.PriPageSudClose ();
			}
			pageHistory.Clear ();
		}

		public void CloseGroupPages (List<string> pageNames)
		{
			while (currPage != null && pageNames.Contains (currPage.pageName)) {
				CloseCurrPage ();
			}
		}

		#endregion

		#region Page Tool

		public T GetPage<T> ()where T:PageBase
		{
			return pageHistory.GetPage<T> ();
		}

		public bool AtlasCouldRelease (string atlasName)
		{
			foreach (var name in AlwaysInMemoryAtlasName) {
				if (atlasName.IndexOf ("-" + name) > 0) {
					return false;
				}
			}
			return true;
		}

		private void _SortCurrGroup ()
		{
			if (pageHistory.LastGroup != null) {
				int sortAdd = 0;
				for (int i = 0, imax = pageHistory.LastGroup.pageQueue.Count; i < imax; i++) {
					var page = pageHistory.GetPage (pageHistory.LastGroup.pageQueue [i]);
					page.canvasData.ResetAddtionalOrder (sortAdd);
					sortAdd = page.canvasData.MaxAddOrder + 10;
				}
			}
		}

		#endregion

		#if UNITY_EDITOR
		static Vector3[] fourCorners = new Vector3[4];

		public static void _OnDrawGizmos ()
		{
			foreach (UnityEngine.UI.MaskableGraphic g in GameObject.FindObjectsOfType<UnityEngine.UI.MaskableGraphic>()) {
				if (g.raycastTarget) {
					RectTransform rectTransform = g.transform as RectTransform;
					rectTransform.GetWorldCorners (fourCorners);
					Gizmos.color = Color.red;
					for (int i = 0; i < 4; i++) {
						Gizmos.DrawLine (fourCorners [i], fourCorners [(i + 1) % 4]);
					}
				}
			}
		}
		#endif
	}
}