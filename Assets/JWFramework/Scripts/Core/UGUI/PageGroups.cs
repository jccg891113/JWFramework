using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace JWFramework.UGUI.Private
{
	[System.Serializable]
	public class PageGroups
	{
		[SerializeField]
		Dictionary<string, PageHistoryItem> allPages;
		[SerializeField]
		QueueList<PageGroup> pageGroups;

		public PageBase LastPage {
			get {
				if (pageGroups.Count > 0) {
					string pageName = pageGroups [pageGroups.Count - 1].lastPage;
					return GetPage (pageName);
				}
				return null;
			}
		}

		public PageGroup LastGroup {
			get {
				if (pageGroups.Count > 0) {
					return pageGroups [pageGroups.Count - 1];
				} else {
					return null;
				}
			}
		}

		public PageGroups ()
		{
			allPages = new Dictionary<string, PageHistoryItem> ();
			pageGroups = new QueueList<PageGroup> ();
		}

		public List<PageBase> AllPages {
			get {
				List<PageBase> res = new List<PageBase> ();
				foreach (var page in allPages.Values) {
					res.Add (page.pageReferenced);
				}
				return res;
			}
		}

		public List<Texture> AllGroupImportantTextures {
			get {
				List<Texture> res = new List<Texture> ();
				for (int i = 0, imax = pageGroups.Count; i < imax; i++) {
					foreach (var pageName in pageGroups[i].pageQueue) {
						var page = GetPage (pageName);
						if ((i == imax - 1) || page.hideType == HideType.OutScreen) {
							foreach (var item in page.textureData.referencedTextures) {
								if (!res.Contains (item)) {
									res.Add (item);
								}
							}
						}
					}
				}
				return res;
			}
		}

		public List<Texture> OtherGroupReleaseTextures {
			get {
				List<Texture> res = new List<Texture> ();
				for (int i = 0, imax = pageGroups.Count - 1; i < imax; i++) {
					foreach (var pageName in pageGroups[i].pageQueue) {
						var page = GetPage (pageName);
						if (page.hideType == HideType.DisableAndRelease) {
							foreach (var item in page.textureData.referencedTextures) {
								if (!res.Contains (item)) {
									res.Add (item);
								}
							}
						}
					}
				}
				return res;
			}
		}

		public List<Texture> AllGroupReleaseTextures {
			get {
				List<Texture> res = new List<Texture> ();
				for (int i = 0, imax = pageGroups.Count; i < imax; i++) {
					foreach (var pageName in pageGroups[i].pageQueue) {
						var page = GetPage (pageName);
//						if (page.hideType == HideType.DisableAndRelease) {
						foreach (var item in page.textureData.referencedTextures) {
							if (!res.Contains (item)) {
								res.Add (item);
							}
						}
//						}
					}
				}
				return res;
			}
		}

		public void Clear ()
		{
			allPages.Clear ();
			pageGroups.Clear ();
		}

		public bool PageInHistory (string pageName)
		{
			return allPages.ContainsKey (pageName);
		}

		public void PageEnqueue (PageBase page)
		{
			if (!allPages.ContainsKey (page.pageName)) {
				allPages [page.pageName] = new PageHistoryItem (page);
			} else {
				allPages [page.pageName].AddPage ();
			}
			if (page.pageType == PageType.FULL_SCREEN) {
				pageGroups.Enqueue (new PageGroup (page.pageName));
			} else {
				if (LastGroup != null) {
					LastGroup.PageEnqueue (page.pageName);
				} else {
					pageGroups.Enqueue (new PageGroup (page.pageName));
				}
			}
		}

		public void LastPageDequeue ()
		{
			if (LastGroup != null) {
				string pageName = LastGroup.PageDequeue ();
				allPages [pageName].RemovePage ();
				if (allPages [pageName].CouldRelease) {
					allPages.Remove (pageName);
				}
				if (LastGroup.pageQueue.Count <= 0) {
					pageGroups.Dequeue ();
				}
			}
		}

		public PageBase GetPage (string pageName)
		{
			if (allPages.ContainsKey (pageName)) {
				return allPages [pageName].pageReferenced;
			}
			return null;
		}

		public T GetPage<T> () where T:PageBase
		{
			T resPage = null;
			foreach (var item in allPages) {
				resPage = item.Value.pageReferenced.GetComponent<T> ();
				if (resPage != null) {
					return resPage;
				}
			}
			return resPage;
		}
	}
}