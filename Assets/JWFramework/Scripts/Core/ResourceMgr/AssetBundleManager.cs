using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using JWFramework.Resource.Assets.Private;

namespace JWFramework.Resource.Assets
{
	public class AssetBundleManager : MonoBehaviour
	{
		const string keyCon = "@&%";

		Dictionary<string, ABResItem> res = new Dictionary<string, ABResItem> ();

		public bool LoadOver {
			get {
				if (res.Count <= 0) {
					return true;
				} else {
					foreach (var item in res.Values) {
						if (!item.loadOver) {
							return false;
						}
					}
					return true;
				}
			}
		}

		public void RegisterLoadAsset (string assetBundlePath, string assetName)
		{
			string _key = GetKey (assetBundlePath, assetName);
			if (!res.ContainsKey (_key)) {
				ABResItem item = new ABResItem (this, assetBundlePath, assetName, false);
				res.Add (_key, item);
			}
		}

		private string GetKey (string assetBundlePath, string assetName)
		{
			return assetBundlePath + keyCon + assetName;
		}

		public void RegisterLoadAsset_Sud (string assetBundlePath, string assetName)
		{
			string _key = GetKey (assetBundlePath, assetName);
			if (!res.ContainsKey (_key)) {
				ABResItem item = new ABResItem (this, assetBundlePath, assetName, true);
				res.Add (_key, item);
			}
		}

		public bool AssetLoadFinished (string assetBundlePath, string assetName)
		{
			string _key = GetKey (assetBundlePath, assetName);
			if (!res.ContainsKey (_key)) {
				return true;
			} else {
				return res [_key].loadOver;
			}
		}

		public Object LoadAsset (string assetBundlePath, string assetName)
		{
			string _key = GetKey (assetBundlePath, assetName);
			if (res.ContainsKey (_key)) {
				if (res [_key].loadOver) {
					return res [_key].AssetObject;
				} else {
					return null;
				}
			} else {
				return null;
			}
		}

		public void UnloadAsset (string assetBundlePath, string assetName)
		{
			string _key = GetKey (assetBundlePath, assetName);
			if (res.ContainsKey (_key)) {
				if (res [_key].Unload ()) {
					res [_key].Clean ();
					res.Remove (_key);
					Resources.UnloadUnusedAssets ();
				}
			}
		}

		public void CleanAll ()
		{
			foreach (var item in res) {
				item.Value.Clean ();
			}
			res.Clear ();
		}
	}
}