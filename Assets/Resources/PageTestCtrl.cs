using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using JWFramework.UGUI;

public class PageTestCtrl : MonoBehaviour
{
	public void OpenPage (int num)
	{
		PageManager.Ins.Open ("Page" + num);
	}

	public void Play ()
	{
		JWFramework.Anim.JWAnimTools.PlayAnim (gameObject, "Anim");
		System.Collections.Generic.List<int> s = new System.Collections.Generic.List<int> ();
	}

	void Start ()
	{
//		List<int> glist = new List<int> ();
//		JWFramework.JWList<int> list = new JWFramework.JWList<int> ();
//		
//		
//		for (int i = 0; i < 100000; i++) {
//			int tmp = Random.Range (int.MinValue, int.MaxValue);
//			glist.Add (tmp);
//			list.Add (tmp);
//		}
//		
//		System.DateTime a = System.DateTime.Now;
//		glist.Sort ((l, r) => {
//			return l.CompareTo (r);
//		});
//		var time1 = System.DateTime.Now - a;
//		a = System.DateTime.Now;
//		list.Sort ((l, r) => {
//			return l.CompareTo (r);
//		});
//		var time2 = System.DateTime.Now - a;
//		Debug.Log ("List:" + time1.TotalMilliseconds);
//		Debug.Log ("JWList:" + time2.TotalMilliseconds);
	}
}
