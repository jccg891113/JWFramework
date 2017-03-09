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
					throw new System.Exception ("Current scene not have PageMgr");
				}
				return _ins;
			}
		}

		[Tooltip ("UI摄像机对应的unity摄像机")]
		public Camera uiCamera;
		
		[Tooltip ("page页面的根挂载点")]
		public Transform pageRoot;

		[SerializeField][Tooltip ("已打开页面队列")]
		private Private.PageGroups pageHistory;

		/// <summary>
		/// 页面堆，用于保存已打开过的常驻内存页面
		/// </summary>
		private Private.PageHeap pageHeap;

		/// <summary>
		/// 当前已打开页面
		/// </summary>
		private PageBase currPage { get { return pageHistory.LastPage; } }

		[SerializeField][Tooltip ("永不释放图集的图集名称")]
		private List<string> AlwaysInMemoryAtlasName;

		void Awake ()
		{
			if (_ins == null) {
				_ins = this;
				pageHistory = new UGUI.Private.PageGroups ();
				pageHeap = new UGUI.Private.PageHeap ();
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
			PageBase currPage = GetPage (pageName);
			OpenMain (currPage, param);
			SortCurrGroup ();
		}

		#endregion

		#region Open - GetPage

		private PageBase GetPage (string pageName)
		{
			PageBase page = GetPageFromMemory (pageName);
			return (page != null) ? page : GetPageFromResource (pageName);
		}

		private PageBase GetPageFromMemory (string pageName)
		{
			PageBase page = pageHistory.GetPage (pageName);
			return (page != null) ? page : pageHeap.GetPage (pageName);
		}

		/// <summary>
		/// 由Resources中加载页面
		/// </summary>
		/// <returns>结果页面</returns>
		/// <param name="pageName">页面名称</param>
		private PageBase GetPageFromResource (string pageName)
		{
			PageBase page = null;
			Object obj = Resources.Load ("Pages/" + pageName);
			if (obj != null) {
				GameObject go = Instantiate (obj) as GameObject;
				go.name = pageName;
				InitPageTransform (go);
				InitPageRectTransform (go);
				page = go.GetComponent<PageBase> ();
				if (page != null) {
					page.SetPageName (pageName);
				} else {
					throw new System.Exception ("Unable load page \'" + pageName + "\'");
				}
			}
			return page;
		}

		private void InitPageTransform (GameObject goPage)
		{
			goPage.transform.SetParent (pageRoot);
			goPage.transform.localPosition = Vector3.zero;
			goPage.transform.localScale = Vector3.one;
		}

		private void InitPageRectTransform (GameObject goPage)
		{
			RectTransform rt = goPage.GetComponent<RectTransform> ();
			rt.anchorMin = Vector2.zero;
			rt.anchorMax = Vector2.one;
			rt.offsetMin = Vector2.zero;
			rt.offsetMax = Vector2.zero;
		}

		#endregion

		#region Open - Main

		private void OpenMain (PageBase currPage, JWData param)
		{
			OperationLastPages (currPage.pageType);
			InitOrSaveParamBeforeOpenPage (currPage);
			PageShow (currPage, param);
		}

		#endregion

		#region Open - Main - Operation Last Pages

		private void OperationLastPages (PageType newPageType)
		{
			if (newPageType == PageType.FULL_SCREEN) {
				HideLastGroup ();
			} else {
				SetCurrentPageBack ();
			}
		}

		private void HideLastGroup ()
		{
			if (pageHistory.LastGroup != null) {
				foreach (var pageName in pageHistory.LastGroup.pageQueue) {
					var page = pageHistory.GetPage (pageName);
					page.PriPageHide ();
				}
			}
		}

		private void SetCurrentPageBack ()
		{
			if (currPage != null) {
				currPage.PriPageBack ();
			}
		}

		#endregion

		#region Open - Main - Init New Page

		private void InitOrSaveParamBeforeOpenPage (PageBase currPage)
		{
			bool firstOpen = IsFirstOpenPage (currPage.pageName);
			if (firstOpen) {
				InitPage (currPage);
			} else {
				SavePageParam (currPage);
			}
		}

		private bool IsFirstOpenPage (string pageName)
		{
			return !pageHistory.PageInHistory (pageName);
		}

		private void InitPage (PageBase currPage)
		{
			currPage.InitFirstOpen ();
		}

		private void SavePageParam (PageBase currPage)
		{
			currPage.SaveParamToHistory ();
		}

		#endregion

		#region Open - Main - Show New Page

		private void PageShow (PageBase page, JWData param)
		{
			PutPageInGroupAndMemory (page);
			page.PriPageOpen (param);
			ReleaseUnusedResourcesAfterOpen ();
		}

		private void PutPageInGroupAndMemory (PageBase page)
		{
			pageHistory.PageEnqueue (page);
		}

		private void ReleaseUnusedResourcesAfterOpen ()
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
				RemovePageInMemoryAndGroup ();
				CloseMain (closePage);
				ReshowOtherPages (closePage);
			}
			SortCurrGroup ();
		}

		public void Close (string pageName)
		{
			PageBase closePage = this.currPage;
			if (closePage != null && closePage.pageName == pageName) {
				RemovePageInMemoryAndGroup ();
				CloseMain (closePage);
				ReshowOtherPages (closePage);
			}
			SortCurrGroup ();
		}

		#endregion

		#region Close - Page Dequeue

		private void RemovePageInMemoryAndGroup ()
		{
			pageHistory.LastPageDequeue ();
		}

		#endregion

		#region Close - Main

		private void CloseMain (PageBase closePage)
		{
			bool pageWillHide = NeedHidePage (closePage);
			if (pageWillHide) {
				HidePage (closePage);
			} else {
				ClosePage (closePage);
			}
			ReleaseResourceAfterClose (closePage, pageWillHide);
		}

		private bool NeedHidePage (PageBase closePage)
		{
			return pageHistory.PageInHistory (closePage.pageName);
		}

		#endregion

		#region Close - Main - Hide Page

		private void HidePage (PageBase closePage)
		{
			LoadHistoryParam (closePage);
			HideClosePage (closePage);
		}

		private void LoadHistoryParam (PageBase closePage)
		{
			closePage.ResetParamFromHistory ();
		}

		private void HideClosePage (PageBase closePage)
		{
			closePage.PriPageHide ();
		}

		#endregion

		#region Close - Main - Close Page

		private void ClosePage (PageBase closePage)
		{
			closePage.TransPageState (PageState.CLOSE);
			closePage.PageClose ();
			ClosePageMain (closePage);
			PutClosePageInHeap (closePage);
		}

		private void ClosePageMain (PageBase closePage)
		{
			if (closePage.closeType == CloseType.Disable) {
				closePage.gameObject.SetActive (false);
			} else {
				Destroy (closePage.gameObject);
			}
		}

		private void PutClosePageInHeap (PageBase closePage)
		{
			if (closePage.closeType == CloseType.Disable && !pageHeap.Contains (closePage.name)) {
				pageHeap.Add (closePage);
			}
		}

		#endregion

		#region Close - Main - Release Resource

		private void ReleaseResourceAfterClose (PageBase closePage, bool pageWillHide)
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

		private void ReshowOtherPages (PageBase closePage)
		{
			if (closePage.pageType == PageType.FULL_SCREEN) {
				ReshowLastGroup ();
			} else {
				if (currPage != null) {
					currPage.PriPageReopen ();
				}
			}
		}

		private void ReshowLastGroup ()
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
				RemovePageInMemoryAndGroup ();
				CloseMain (closePage);
			}
			// open
			PageBase currPage = GetPage (pageName);
			CloseAndOpenOperationLastPages (closePage, currPage);
			InitOrSaveParamBeforeOpenPage (currPage);
			PageShow (currPage, param);
			// sort
			SortCurrGroup ();
		}

		private void CloseAndOpenOperationLastPages (PageBase closePage, PageBase newPage)
		{
			if (closePage != null) {
				if (newPage.pageType == PageType.FULL_SCREEN) {
					HideLastGroup ();
				} else {
					BackLastGroup ();
				}
			}
		}

		private void BackLastGroup ()
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
				bool important = importantTextures.Contains (couldReleaseTextures [i]);
				if (!important) {
					Resources.UnloadAsset (couldReleaseTextures [i]);
				}
			}
		}

		#region Close All

		public void CloseAll ()
		{
			CloseAllLastPage ();
			CloseAllReleaseResource ();
			CloseAllMain ();
		}

		private void CloseAllLastPage ()
		{
			PageBase closePage = this.currPage;
			if (closePage != null) {
				closePage.PriPageClose ();
			}
		}

		private void CloseAllReleaseResource ()
		{
			List<Texture> releaseTextures = pageHistory.AllGroupReleaseTextures;
			ReleaseResource (new List<Texture> (), releaseTextures);
		}

		private void CloseAllMain ()
		{
			foreach (var page in pageHistory.AllPages) {
				page.PriPageSudClose ();
			}
			pageHistory.Clear ();
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
				if (atlasName.IndexOf ("-" + name + "-") > 0) {
					return false;
				}
			}
			return true;
		}

		private void SortCurrGroup ()
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

		void OnDrawGizmos ()
		{
			foreach (UnityEngine.UI.MaskableGraphic g in GameObject.FindObjectsOfType<UnityEngine.UI.MaskableGraphic>()) {
				if (g.raycastTarget) {
					RectTransform rectTransform = g.transform as RectTransform;
					rectTransform.GetWorldCorners (fourCorners);
					Gizmos.color = Color.red;
					for (int i = 0; i < 4; i++) {
//						Debug.DrawLine (fourCorners [i], fourCorners [(i + 1) % 4], Color.red);
						Gizmos.DrawLine (fourCorners [i], fourCorners [(i + 1) % 4]);
					}
				}
			}
		}
		#endif
	}
}