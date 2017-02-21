using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace JWFramework.UGUI.Private
{
	public class PageTextureData
	{
		public List<Texture> referencedTextures;

		public PageTextureData (Image[] allImage)
		{
			referencedTextures = new List<Texture> ();
			for (int i = 0, imax = allImage.Length; i < imax; i++) {
				var texture = allImage [i].mainTexture;
				if (!referencedTextures.Contains (texture)) {
					referencedTextures.Add (texture);
				}
			}
		}
	}
}