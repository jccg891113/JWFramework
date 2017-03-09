using UnityEngine;
using System.Collections;

namespace JWFramework.UGUI.Private
{
	public class PageHistoryItem
	{
		public PageBase pageReferenced;
		private int referencedCount;

		public bool CouldRelease {
			get {
				return referencedCount <= 0;
			}
		}

		public PageHistoryItem (PageBase page)
		{
			pageReferenced = page;
			referencedCount = 1;
		}

		public void AddPage ()
		{
			referencedCount++;
		}

		public void RemovePage ()
		{
			referencedCount--;
		}
	}
}