using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace JWFramework.UGUI.Private
{
	public static class PageManagerFunc
	{
		public static void PriPageOpen (this PageBase page, JWData param)
		{
			page.SetOpenParam (param);
			page.TransPageState (PageState.OPEN);
			page.transform.localPosition = Vector3.zero;
			page.gameObject.SetActive (true);
			page.PageOpen ();
		}

		public static void PriPageReopen (this PageBase page)
		{
			page.TransPageState (PageState.OPEN);
			page.transform.localPosition = Vector3.zero;
			page.gameObject.SetActive (true);
			page.PageOpen ();
		}

		public static void PriPageBack (this PageBase page)
		{
			page.TransPageState (PageState.BACKGROUND);
			page.transform.localPosition = Vector3.zero;
			page.gameObject.SetActive (true);
			page.PageBack ();
		}

		public static void PriPageHide (this PageBase page)
		{
			page.TransPageState (PageState.HIDE);
			if (page.hideType == HideType.OutScreen) {
				page.transform.localPosition = new Vector3 (3000, 0, 0);
			} else {
				page.gameObject.SetActive (false);
			}
			page.PageHide ();
		}

		public static void PriPageClose (this PageBase page)
		{
			page.TransPageState (PageState.CLOSE);
			page.PageClose ();
			if (page.closeType == CloseType.Disable) {
				page.gameObject.SetActive (false);
			} else {
				MonoBehaviour.Destroy (page.gameObject);
			}
		}

		public static void PriPageSudClose (this PageBase page)
		{
			page.TransPageState (PageState.CLOSE);
			if (page.closeType == CloseType.Disable) {
				page.gameObject.SetActive (false);
			} else {
				MonoBehaviour.Destroy (page.gameObject);
			}
		}
	}
}