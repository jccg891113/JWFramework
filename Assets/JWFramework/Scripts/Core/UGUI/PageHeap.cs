using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace JWFramework.UGUI.Private
{
	public class PageHeap
	{
		private Dictionary<string, PageBase> pageHeap = new Dictionary<string, PageBase> ();

		public PageHeap ()
		{
			pageHeap = new Dictionary<string, PageBase> ();
		}

		public void Clear ()
		{
			foreach (var item in pageHeap) {
				MonoBehaviour.Destroy (item.Value);
			}
			pageHeap.Clear ();
		}

		public void Add (PageBase page)
		{
			pageHeap [page.pageName] = page;
		}

		public bool Contains (string name)
		{
			return pageHeap.ContainsKey (name);
		}

		public PageBase GetPage (string pageName)
		{
			if (pageHeap.ContainsKey (pageName)) {
				return pageHeap [pageName];
			}
			return null;
		}
	}
}