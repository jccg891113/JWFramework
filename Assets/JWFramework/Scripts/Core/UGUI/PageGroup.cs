using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace JWFramework.UGUI.Private
{
	[System.Serializable]
	public class PageGroup
	{
		public QueueList<string> pageQueue;

		public string lastPage {
			get {
				if (pageQueue.Count > 0) {
					return pageQueue [pageQueue.Count - 1];
				} else {
					return "";
				}
			}
		}

		public string firstPage {
			get {
				if (pageQueue.Count > 0) {
					return pageQueue [0];
				} else {
					return "";
				}
			}
		}

		public PageGroup (string firstPage)
		{
			pageQueue = new QueueList<string> ();
			pageQueue.Enqueue (firstPage);
		}

		public void PageEnqueue (string newPage)
		{
			pageQueue.Enqueue (newPage);
		}

		public string PageDequeue ()
		{
			return pageQueue.Dequeue ();
		}
	}
}