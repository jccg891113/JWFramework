using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using JWFramework.Resource.Assets.Private;

namespace JWFramework.Resource.Assets
{
	public class ResourcesManager : MonoBehaviour
	{
		Dictionary<string, RResItem> res = new Dictionary<string, RResItem> ();

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

		public void PreLoadAsset (string assetName)
		{
			if (!res.ContainsKey (assetName)) {
				var item = new RResItem (this, assetName, false);
				res.Add (assetName, item);
			}
		}

		public void PreLoadAsset_Sud (string assetName)
		{
			if (!res.ContainsKey (assetName)) {
				var item = new RResItem (this, assetName, true);
				res.Add (assetName, item);
			}
		}

		public bool AssetLoadFinished (string assetName)
		{
			if (!res.ContainsKey (assetName)) {
				return true;
			} else {
				return res [assetName].loadOver;
			}
		}

		public Object LoadAsset (string assetName)
		{
			if (res.ContainsKey (assetName)) {
				if (res [assetName].loadOver) {
					return res [assetName].AssetObject;
				} else {
					return null;
				}
			} else {
				return null;
			}
		}

		public void UnloadAsset (string assetName)
		{
			if (res.ContainsKey (assetName)) {
				if (res [assetName].Unload ()) {
					res [assetName].Clean ();
					res.Remove (assetName);
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