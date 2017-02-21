using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class NewBehaviourScript : MonoBehaviour
{
	public List<Texture> referencedTextures;
	// Use this for initialization
	void Start ()
	{
		Image[] allImage = GetComponentsInChildren<Image> ();
		referencedTextures = new List<Texture> ();
		for (int i = 0, imax = allImage.Length; i < imax; i++) {
			var texture = allImage [i].mainTexture;
			if (!referencedTextures.Contains (texture)) {
				referencedTextures.Add (texture);
			}
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}

	public void Cli ()
	{
		foreach (var item in referencedTextures) {
			Resources.UnloadAsset (item);
		}
	}

	public void Show ()
	{
		gameObject.SetActive (true);
	}

	public void Hide ()
	{
		gameObject.SetActive (false);
	}
}
