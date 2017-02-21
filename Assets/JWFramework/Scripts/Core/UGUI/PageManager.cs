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
		public Camera mainCamera;

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
				go.transform.parent = transform;
				go.transform.localPosition = Vector3.zero;
				go.transform.localScale = Vector3.one;
				go.name = pageName;
				page = go.GetComponent<PageBase> ();
				if (page != null) {
					page.SetPageName (pageName);
				} else {
					throw new System.Exception ("Unable load page \'" + pageName + "\'");
				}
			}
			return page;
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
			List<Texture> currGroupTextures = pageHistory.CurrGroupTextures;
			List<Texture> otherGroupTextures = pageHistory.OtherGroupReleaseTextures;
			ReleaseResource (currGroupTextures, otherGroupTextures);
		}

		#endregion

		#region Close

		public void Close ()
		{
			PageBase closePage = this.currPage;
			if (closePage != null) {
				RemovePageInMemoryAndGroup ();
				CloseMain (closePage);
				ReshowOtherPages (closePage);
			}
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
			if (NeedHidePage (closePage)) {
				HidePage (closePage);
			} else {
				ClosePage (closePage);
			}
			ReleaseResourceAfterClose (closePage);
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

		private void ReleaseResourceAfterClose (PageBase closePage)
		{
			List<Texture> currGroupTextures = pageHistory.CurrGroupTextures;
			List<Texture> otherGroupTextures = pageHistory.OtherGroupReleaseTextures;
			for (int i = 0, imax = closePage.textureData.referencedTextures.Count; i < imax; i++) {
				if (!otherGroupTextures.Contains (closePage.textureData.referencedTextures [i])) {
					otherGroupTextures.Add (closePage.textureData.referencedTextures [i]);
				}
			}
			ReleaseResource (currGroupTextures, otherGroupTextures);
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

		private void ReleaseResource (List<Texture> importantTextures, List<Texture> couldReleaseTextures)
		{
			for (int i = 0, imax = couldReleaseTextures.Count; i < imax; i++) {
				bool couldUnload = !importantTextures.Contains (couldReleaseTextures [i]);
				if (couldUnload) {
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

		#endregion
	}
}