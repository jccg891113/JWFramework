using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using JWFramework.Tools;

namespace JWFramework.Resource.Pool
{
	public class GameObjectPool
	{
		public string resName;
		private Object prefab;
		private int minCount;
		private int totalCound;
		public List<GameObject> resPool = new List<GameObject> ();

		public GameObjectPool (string resName, Object resource, int count)
		{
			this.resName = resName;
			this.resPool.Clear ();
			this.prefab = resource;
			this.minCount = count;
			if (prefab != null) {
				for (int i = 0; i < count; i++) {
					GameObject res = InstantiatePrefab ();
					resPool.Add (res);
				}
				totalCound = count;
			}
		}

		private GameObject InstantiatePrefab ()
		{
			GameObject res = MonoBehaviour.Instantiate (prefab) as GameObject;
			var comp = res.AddMissingComponent<GOItem> ();
			comp.ResourceInit (resName);
			res.transform.parent = ObjectPools.Default.transform;
			res.transform.localPosition = Vector3.zero;
			res.SetActive (false);
			return res;
		}

		public void Reload (int count)
		{
			for (int i = 0; i < count; i++) {
				GameObject res = InstantiatePrefab ();
				resPool.Add (res);
			}
			totalCound += count;
		}

		public GameObject Spawn ()
		{
			if (resPool.Count <= 0) {
				totalCound++;
				GameObject res = InstantiatePrefab ();
				return res;
			} else {
				int last = resPool.Count;
				GameObject res = resPool [last - 1];
				resPool.RemoveAt (last - 1);
				return res;
			}
		}

		public void Recovery (GameObject go)
		{
			if (!resPool.Contains (go)) {
				go.transform.parent = ObjectPools.Default.transform;
				go.transform.localPosition = Vector3.zero;
				go.SetActive (false);
				resPool.Add (go);
			}
		}

		public void Clean ()
		{
			totalCound = 0;
			foreach (GameObject obj in resPool) {
				if (obj != null) {
					GameObject.Destroy (obj);
				}
			}
			resPool.Clear ();
		}

		public void AutoRelease ()
		{
			if (totalCound > minCount) {
				int halfCount = Mathf.CeilToInt ((totalCound - minCount) * 0.5f);
				if (resPool.Count > halfCount) {
					for (int i = 0; i < halfCount; i++) {
						int last = resPool.Count;
						GameObject obj = resPool [last - 1];
						resPool.RemoveAt (last - 1);
						MonoBehaviour.Destroy (obj);
					}
					totalCound -= halfCount;
				}
			}
		}
	}
}