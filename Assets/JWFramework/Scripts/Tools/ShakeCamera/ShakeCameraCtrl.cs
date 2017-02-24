using UnityEngine;
using System.Collections;

namespace JWFramework.Tools
{
	public class ShakeCameraCtrl : MonoBehaviour
	{
		public float startTime = 0;
		public float duration = 0;
		public float strength = 1f;
		public bool isUnitSphere = true;
		public bool needX = false;
		public bool needY = false;
		public bool needZ = false;
		public AnimationCurve shakeCurve = new AnimationCurve (new Keyframe (0, 1, 0, 0), new Keyframe (1, 1, 0, 0));
	
		private float scriptTime = 0;
		private float scriptShakeTime = 0;
		private bool playing = true;
	
		ShakeCameraKit shakeKit;
		// Use this for initialization
		void Start ()
		{
			if (shakeKit == null) {
				shakeKit = GetComponent<ShakeCameraKit> ();
			}
		}

		// Update is called once per frame
		void Update ()
		{
			if (playing) {
				scriptTime += Time.deltaTime;
				if (scriptTime >= startTime) {
					scriptShakeTime += Time.deltaTime;
					shakeKit.PlayShake ();
					if (scriptShakeTime >= duration) {
						playing = false;
					}
				}
			}
		}
	}
}