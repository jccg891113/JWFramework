using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace JWFramework.Anim
{
	[System.Serializable]
	public class JWAnimBase
	{
		#region

		[Header ("Type")]
		public UIAnimType animType;

		[Header ("All Type Base Attribute")]
		public UIAnimPlayMode playMode;
		public float delay;
		public float duration;
		public AnimationCurve curve = new AnimationCurve (new Keyframe (0, 0), new Keyframe (1, 1));

		[Header ("HelperAttribute")]
		public Vector3 beginVec3;
		public Vector3 endVec3;
		public bool scaleOfX = true;
		public bool scaleOfY = true;
		public bool scaleOfZ = true;
		public UnityEngine.UI.Image image;
		public List<CanvasGroup> alphaGroup = new List<CanvasGroup> ();
		public Color beginColor = Color.white;
		public Color endColor = Color.white;
		public List<UnityEngine.UI.Graphic> colorGroup = new List<UnityEngine.UI.Graphic> ();

		#endregion

		private enum AnimState
		{
			delay,
			play,
			end,
		}

		private float realDeltaTime;
		private AnimState state;

		public bool isOver {
			get {
				return state == AnimState.end;
			}
		}

		public void BeginPlay ()
		{
			realDeltaTime = 0;

			if (delay > 0) {
				state = AnimState.delay;
			} else {
				state = AnimState.play;
			}

			switch (playMode) {
			case UIAnimPlayMode.Once:
				curve.postWrapMode = WrapMode.Clamp;
				break;
			case UIAnimPlayMode.Loop:
				curve.postWrapMode = WrapMode.Loop;
				break;
			case UIAnimPlayMode.PingPong:
				curve.postWrapMode = WrapMode.PingPong;
				break;
			}
		}

		#region Update

		public void Update (Transform t, float delta)
		{
			realDeltaTime += delta;
			switch (state) {
			case AnimState.delay:
				if (realDeltaTime >= delay) {
					state = AnimState.play;
					realDeltaTime = 0;
				}
				break;
			case AnimState.play:
				float p = Evaluate ();
				UpdateKit (t, p);
				if (playMode == UIAnimPlayMode.Once) {
					if (realDeltaTime >= duration) {
						state = AnimState.end;
					}
				}
				break;
			}
		}

		private void UpdateKit (Transform t, float p)
		{
			switch (animType) {
			case UIAnimType.Position:
				PositionUpdate (t, p);
				break;
			case UIAnimType.Euler:
				EulerUpdate (t, p);
				break;
			case UIAnimType.Scale:
				ScaleUpdate (t, p);
				break;
			case UIAnimType.UGUI_Alpha:
				AlphaUpdate (p);
				break;
			case UIAnimType.UGUI_ImageFilled:
				ImageFilledUpdate (p);
				break;
			case UIAnimType.UGUI_Color:
				ColorUpdate (p);
				break;
			}
		}

		private float Evaluate ()
		{
			float lifeTime = duration;
			float currTime = realDeltaTime;
			float computeTime = currTime / lifeTime;
			return curve.Evaluate (computeTime);
		}

		private void PositionUpdate (Transform t, float p)
		{
			t.localPosition = p * endVec3 + (1 - p) * beginVec3;
		}

		private void EulerUpdate (Transform t, float p)
		{
			t.localEulerAngles = p * endVec3 + (1 - p) * beginVec3;
		}

		private void ScaleUpdate (Transform t, float p)
		{
			float x = scaleOfX ? p : t.localScale.x;
			float y = scaleOfY ? p : t.localScale.y;
			float z = scaleOfZ ? p : t.localScale.z;
			t.localScale = new Vector3 (x, y, z);
		}

		private void AlphaUpdate (float p)
		{
			if (alphaGroup != null) {
				foreach (var item in alphaGroup) {
					if (item != null) {
						item.alpha = p;
					}
				}
			}
		}

		private void ImageFilledUpdate (float p)
		{
			image.fillAmount = p;
		}

		private void ColorUpdate (float p)
		{
			if (colorGroup != null) {
				foreach (var item in colorGroup) {
					if (item != null) {
						item.color = Color.Lerp (beginColor, endColor, p);
					}
				}
			}
		}

		#endregion

		#region Begin Value

		public void SetToBeginValue (Transform t)
		{
			float p = curve.Evaluate (0);
			UpdateKit (t, p);
		}

		#endregion

		#region Begin Value

		public void SetToEndValue (Transform t)
		{
			float p = curve.Evaluate (1);
			UpdateKit (t, p);
		}

		#endregion
	}

	public enum UIAnimType
	{
		None,
		Position,
		Euler,
		Scale,
		UGUI_Alpha,
		UGUI_ImageFilled,
		UGUI_Color,
	}

	public enum UIAnimPlayMode
	{
		Once,
		Loop,
		PingPong,
	}
}