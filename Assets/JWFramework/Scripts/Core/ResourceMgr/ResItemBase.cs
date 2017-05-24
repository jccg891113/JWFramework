using UnityEngine;
using System.Collections;

namespace JWFramework.Resource.Assets.Private
{
	public class ResItemBase
	{
		protected string assetBundlePath;
		protected string assetName;

		public bool loadOver{ get; protected set; }

		public bool loadSuccess{ get; protected set; }

		public int referenceCount{ get; protected set; }

		protected Object assetObject;

		public Object AssetObject {
			get {
				referenceCount++;
				return assetObject;
			}
		}

		public ResItemBase (MonoBehaviour mono, string assetBundlePath, string assetName, bool isSud)
		{
			this.assetBundlePath = assetBundlePath;
			this.assetName = assetName;
			this.assetObject = null;
			this.loadOver = false;
			this.loadSuccess = false;
			this.referenceCount = 0;
			if (isSud) {
				LoadSud ();
			} else {
				mono.StartCoroutine (_Load ());
			}
		}

		protected virtual void LoadSud ()
		{
		}

		protected virtual IEnumerator _Load ()
		{
			yield return new WaitForEndOfFrame ();
		}

		public bool Unload ()
		{
			referenceCount--;
			return referenceCount <= 0;
		}

		public void Clean ()
		{
//			try {
//				Resources.UnloadAsset (assetObject);
//			} catch {
//			}
			assetObject = null;
		}
	}
}