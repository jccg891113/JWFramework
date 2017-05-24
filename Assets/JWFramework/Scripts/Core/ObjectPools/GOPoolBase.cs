using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using JWFramework.Tools;

namespace JWFramework.Resource.Pool
{
	[System.Serializable]
	public class GOPoolBase
	{
		protected Object prefab;
		[SerializeField]
		protected int minCount;
		[SerializeField]
		protected int totalCound;
		[SerializeField]
		protected List<GameObject> resPool = new List<GameObject> ();
		[SerializeField]
		protected Transform poolTransform;

		protected GOPoolBase (Transform poolTransform)
		{
			this.poolTransform = poolTransform;
		}

		public GOPoolBase (Object resource, int count, Transform poolTransform)
		{
			this.resPool.Clear ();
			this.prefab = resource;
			this.minCount = count;
			this.poolTransform = poolTransform;
			if (prefab != null) {
				for (int i = 0; i < count; i++) {
					GameObject res = InstantiatePrefab ();
					resPool.Add (res);
				}
				totalCound = count;
			}
		}

		protected GameObject InstantiatePrefab ()
		{
			GameObject res = MonoBehaviour.Instantiate (prefab) as GameObject;
			this.WhenInstantiatePrefab (res);
			res.transform.SetParent (poolTransform);
			res.transform.localPosition = Vector3.zero;
			res.SetActive (false);
			return res;
		}

		protected virtual void WhenInstantiatePrefab (GameObject res)
		{
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
				int lastId = resPool.Count - 1;
				GameObject res = resPool [lastId];
				resPool.RemoveAt (lastId);
				return res;
			}
		}

		public void Recovery (GameObject go)
		{
			if (!resPool.Contains (go)) {
				go.transform.SetParent (poolTransform);
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