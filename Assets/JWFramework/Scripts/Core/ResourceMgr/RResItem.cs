using UnityEngine;
using System.Collections;

namespace JWFramework.Resource.Assets.Private
{
	public class RResItem : ResItemBase
	{
		public RResItem (MonoBehaviour mono, string assetName, bool isSud) : base (mono, "", assetName, isSud)
		{
		}

		protected override void LoadSud ()
		{
			this.assetObject = Resources.Load (assetName);
			loadOver = true;
		}

		protected override IEnumerator _Load ()
		{
			var res = Resources.LoadAsync (assetName);
			yield return res;
			this.assetObject = res.asset;
			loadOver = true;
			loadSuccess = true;
		}
	}
}