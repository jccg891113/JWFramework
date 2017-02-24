using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class JW_EffectUVAnim : MonoBehaviour
{
	public float delayTime;
	public float offsetXSpeed;
	public float offsetYSpeed;
	
	public bool needColor;
	public bool tintColor;
	public AnimationCurve colorR;
	public AnimationCurve colorG;
	public AnimationCurve colorB;
	public AnimationCurve colorA;
	
	private List<Material> allMats = new List<Material> ();
	private bool delayStep;
	private float delta;
	// Use this for initialization
	void Start ()
	{
		delayStep = (delayTime > 0);
		delta = 0;
		allMats.Clear ();
		var renderer = GetComponent<Renderer> ();
		allMats = new List<Material> (renderer.materials);
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (delayStep) {
			if (delta >= delayTime) {
				delayStep = false;
				delta = 0;
			}
		} else {
			Vector2 off = new Vector2 (delta * offsetXSpeed, delta * offsetYSpeed);
			foreach (var item in allMats) {
				item.SetTextureOffset ("_MainTex", off);
			}
			if (needColor) {
				Color newColor = new Color (colorR.Evaluate (delta), colorG.Evaluate (delta), colorB.Evaluate (delta), colorA.Evaluate (delta));
				if (tintColor) {
					foreach (var item in allMats) {
						item.SetColor ("_TintColor", newColor);
					}
				} else {
					foreach (var item in allMats) {
						item.SetColor ("_Color", newColor);
					}
				}
			}
		}
		delta += Time.deltaTime;
	}
}
