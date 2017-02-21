using UnityEngine;
using System.Collections;

namespace JWFramework.UGUI.Private
{
	public class PageHistoryItem
	{
		public PageBase pageReferenced;
		private int referencedCoount;

		public bool CouldRelease {
			get {
				return referencedCoount <= 0;
			}
		}

		public PageHistoryItem (PageBase page)
		{
			pageReferenced = page;
			referencedCoount = 1;
		}

		public void AddPage ()
		{
			referencedCoount++;
		}

		public void RemovePage ()
		{
			referencedCoount--;
		}
	}
}