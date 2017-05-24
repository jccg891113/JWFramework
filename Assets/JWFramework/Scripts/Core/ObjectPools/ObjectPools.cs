using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using JWFramework.Tools;
using JWFramework.Resource.Pool.Private;

namespace JWFramework.Resource.Pool
{
	public class ObjectPools : MonoBehaviour
	{
		private static ObjectPools _instance;

		public static ObjectPools Default {
			get {
				if (_instance == null) {
					_instance = MonoInstanceGenerater<ObjectPools>.GenerateIns (new Vector3 (10000, 0, 0));
					
//					GameObject go = new GameObject ("JWFramework_GameObjectPool");
//					go.transform.localScale = Vector3.one;
//					go.transform.position = new Vector3 (10000, 0, 0);
//					_instance = go.AddComponent<ObjectPools> ();
//					DontDestroyOnLoad (go);
				}
				return _instance;
			}
		}

		float _autoReleasePoolTime;
		public float autoReleasePoolTime = 10;
		private Dictionary<string, GameObjectPool> pool = new Dictionary<string, GameObjectPool> ();

		public void PreLoad (string resName, Object resource, int count)
		{
			if (resource == null) {
				JWDebug.LogError ("GameObject pool error because the Object of resource \"" + resName + "\" is NAN");
			} else {
				if (!pool.ContainsKey (resName)) {
					pool.Add (resName, new GameObjectPool (resName, resource, count, this));
				}
			}
		}

		public GameObject Spawn (string resName)
		{
			if (pool.ContainsKey (resName)) {
				return pool [resName].Spawn ();
			}
			return null;
		}

		public void Recovery (string resName, GameObject go)
		{
			if (pool.ContainsKey (resName)) {
				pool [resName].Recovery (go);
			}
		}

		public void Recovery (GameObject go)
		{
			GOItem item = go.GetComponent<GOItem> ();
			if (item != null && pool.ContainsKey (item.resourceName)) {
				pool [item.resourceName].Recovery (go);
			}
		}

		public void ReloadGameObject (string resName, int count)
		{
			if (pool.ContainsKey (resName)) {
				pool [resName].Reload (count);
			}
		}

		public void Clean (string resName)
		{
			if (pool.ContainsKey (resName)) {
				pool [resName].Clean ();
			}
		}

		public void CleanAll ()
		{
			foreach (var item in pool) {
				item.Value.Clean ();
			}
			pool.Clear ();
		}

		public IEnumerator CleanAllAsyc ()
		{
			System.DateTime clock = System.DateTime.Now;
			foreach (var item in pool) {
				item.Value.Clean ();
				if ((System.DateTime.Now - clock).TotalMilliseconds > 50) {
					clock = System.DateTime.Now;
					yield return new WaitForEndOfFrame ();
				}
			}
			pool.Clear ();
		}

		void Update ()
		{
			_autoReleasePoolTime += Time.deltaTime;
			if (_autoReleasePoolTime > autoReleasePoolTime) {
				foreach (var item in pool.Values) {
					item.AutoRelease ();
				}
				_autoReleasePoolTime = 0;
			}
		}
	}
}