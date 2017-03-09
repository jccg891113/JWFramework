using UnityEngine;
using System.Collections;

namespace JWFramework.Tools
{
	public class ShakeCameraKit : MonoBehaviour
	{
		public float duration = 0;
		public float strength = 1f;
		public bool isUnitSphere = true;
		public bool needX = false;
		public bool needY = false;
		public bool needZ = false;
		public AnimationCurve shakeCurve = new AnimationCurve (new Keyframe (0, 1, 0, 0), new Keyframe (1, 1, 0, 0));
	
		private Vector3 deltaPos = Vector3.zero;
		private bool playing = false;
		private float shakeTime = 0;

		void Start ()
		{
			shakeTime = 0;
		}

		// Update is called once per frame
		void Update ()
		{
			if (playing) {
				shakeTime += Time.deltaTime;
				if (shakeTime < duration) {
					transform.localPosition -= deltaPos;
					float curveP = shakeCurve.Evaluate (shakeTime / duration);
					if (isUnitSphere) {
						deltaPos = Random.insideUnitSphere * strength;
					} else {
						float x = 0, y = 0, z = 0;
						if (needX) {
							x = -1f + 2f * Random.value;
						}
						if (needY) {
							y = -1f + 2f * Random.value;
						}
						if (needZ) {
							z = -1f + 2f * Random.value;
						}
						deltaPos = new Vector3 (x, y, z);
					}
					transform.localPosition += deltaPos;
				} else {
					transform.localPosition -= deltaPos;
					deltaPos = Vector3.zero;
					playing = false;
				}
			}
		}

		public void PlayShake ()
		{
			shakeTime = 0;
			playing = true;
			deltaPos = Vector3.zero;
		}

		public void SetParam (ShakeCameraCtrl ctrl)
		{
			playing = false;
			deltaPos = Vector3.zero;
			this.duration = ctrl.duration;
			this.strength = ctrl.strength;
			this.isUnitSphere = ctrl.isUnitSphere;
			this.needX = ctrl.needX;
			this.needY = ctrl.needY;
			this.needZ = ctrl.needZ;
			this.shakeCurve = ctrl.shakeCurve;
		}

		public static void ImShakeCamera (GameObject obj)
		{
			ShakeCameraKit ctrl = obj.AddMissingComponent<ShakeCameraKit> ();
			ctrl.PlayShake ();
		}
	}
}