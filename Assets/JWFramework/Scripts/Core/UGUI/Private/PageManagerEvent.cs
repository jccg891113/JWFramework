using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace JWFramework.UGUI
{
	public class PageManagerEvent
	{
		/// <summary>
		/// The notification when page manager history info changed
		/// 		[Key]						[Value]
		/// 		GroupCount					Group count
		/// 		CurrGroupRootPageIsFullScreen
		/// </summary>
		public const string UGUI_PAGEMANAGER_QUEUECHANGE = "UGUI_PAGEMANAGER_QUEUECHANGE";

		public static void UGUI_PageManager_QueueChange (Private.PageGroups pageHistory)
		{
			bool isFullScreen = false;
			if (pageHistory.PageGroupCount > 0) {
				var page = pageHistory.GetPage (pageHistory.LastGroup.firstPage);
				isFullScreen = (page.pageType == PageType.FULL_SCREEN);
			}
			JWFramework.NotificationCenter.Default.PostNotification (UGUI_PAGEMANAGER_QUEUECHANGE, null, new JWData ("GroupCount", pageHistory.PageGroupCount, "CurrGroupRootPageIsFullScreen", isFullScreen));
		}
	}
}