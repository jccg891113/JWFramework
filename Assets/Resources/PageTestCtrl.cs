using UnityEngine;
using System.Collections;
using JWFramework.UGUI;

public class PageTestCtrl : MonoBehaviour
{
	public void OpenPage (int num)
	{
		PageManager.Ins.Open ("Page" + num);
	}
}
