using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using System.Collections.Generic;

public class NewEditorTest
{

	[Test]
	public void EditorTest ()
	{
		List<byte[]> testArray = new List<byte[]> ();
		testArray.Add (System.Text.Encoding.ASCII.GetBytes ("a"));
		testArray.Add (System.Text.Encoding.ASCII.GetBytes ("bb"));
		testArray.Add (System.Text.Encoding.ASCII.GetBytes ("ccc"));
		testArray.Add (System.Text.Encoding.ASCII.GetBytes ("dddd"));
		
		byte[] res = { };
		while (testArray.Count > 0) {
			var receiveData = testArray [0];
			Debug.Log (receiveData.Length);
			byte[] tmp = new byte[res.Length + receiveData.Length];
			System.Buffer.BlockCopy (res, 0, tmp, 0, res.Length);
			System.Buffer.BlockCopy (receiveData, 0, tmp, res.Length, receiveData.Length);
			res = new byte[tmp.Length];
			System.Buffer.BlockCopy (tmp, 0, res, 0, tmp.Length);
			
			testArray.RemoveAt (0);
		}
		
		Debug.Log (res.Length);
		
		System.TimeSpan ts = new System.TimeSpan (25, 1, 1);
		Debug.Log (string.Format ("{0:00}:{1:00}:{2:00}", ts.TotalHours, ts.Minutes, ts.Seconds));
		
		Assert.IsNotEmpty (res);
		
	}
}
