using UnityEngine;
using System.Collections;
using JWFramework.UGUI;

public class PageTestBase : PageBase
{
	public void OpenPage (int num)
	{
		PageManager.Ins.Open ("Page" + num);
	}
}
