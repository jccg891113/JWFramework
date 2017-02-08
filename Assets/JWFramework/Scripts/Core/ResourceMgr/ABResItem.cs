using UnityEngine;
using System.Collections;

namespace JWFramework.Resource.Assets.Private
{
	public class ABResItem : ResItemBase
	{
		public ABResItem (MonoBehaviour mono, string assetBundleNAme, string assetName, bool isSud) : base (mono, assetBundleNAme, assetName, isSud)
		{
		}

		protected override void LoadSud ()
		{
			AssetBundle asset = AssetBundle.LoadFromFile (assetBundlePath);
			if (asset != null) {
				assetObject = asset.LoadAsset (assetName);
				if (assetObject != null) {
					loadSuccess = true;
				}
				asset.Unload (false);
			}
			loadOver = true;
		}

		protected override IEnumerator _Load ()
		{
			WWW www = new WWW ("file:///" + assetBundlePath);
			yield return www;
			if (string.IsNullOrEmpty (www.error)) {
				if (www.assetBundle != null) {
					loadSuccess = true;
					assetObject = www.assetBundle.LoadAsset (assetName);
					www.assetBundle.Unload (false);
				} else {
					Debug.LogError ("AssetBundle is NONE!");
					loadSuccess = false;
					assetObject = null;
				}
			} else {
				Debug.LogError (www.error);
				loadSuccess = false;
				assetObject = null;
			}
			loadOver = true;
		}

		private string GetAssetBundlePath (string path)
		{
			#if UNITY_EDITOR
			return "file:///" + path;
			#elif UNITY_IOS
			return "file:///" + path;
			#elif UNITY_ANDROID
			return path;
			#else
			return "file:///" + path;
			#endif
		}
	}
}