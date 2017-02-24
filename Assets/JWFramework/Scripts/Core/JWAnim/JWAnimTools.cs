using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace JWFramework.Anim
{
	public class JWAnimTools : MonoBehaviour
	{
		private bool beginPlay = false;
		private bool beginPlayJumpFirstUpdate = false;
		public string AnimGroupName = "";
		public List<JWAnimBase> anims;

		public bool isOver {
			get {
				if (anims != null) {
					foreach (var item in anims) {
						if (item != null && !item.isOver) {
							return false;
						}
					}
				}
				return true;
			}
		}

		public void Play ()
		{
			beginPlay = true;
			beginPlayJumpFirstUpdate = true;
			foreach (var item in anims) {
				if (item != null) {
					item.BeginPlay ();
				}
			}
		}

		void Update ()
		{
			if (beginPlay) {
				if (beginPlayJumpFirstUpdate) {
					beginPlayJumpFirstUpdate = false;
					return;
				}
				float delta = Time.smoothDeltaTime;
				if (delta > 0.07f) {
					delta = 0.07f;
				}
				if (anims != null) {
					foreach (var item in anims) {
						if (item != null) {
							item.Update (transform, delta);
						}
					}
				}
			}
		}

		public void Pause ()
		{
			beginPlay = false;
		}

		public void Resume ()
		{
			beginPlay = true;
		}

		public void Stop ()
		{
			beginPlay = false;
			SetToEndValue ();
		}

		public void SetToBeginValue ()
		{
			if (anims != null) {
				foreach (var item in anims) {
					if (item != null) {
						item.SetToBeginValue (transform);
					}
				}
			}
		}

		public void SetToEndValue ()
		{
			if (anims != null) {
				foreach (var item in anims) {
					if (item != null) {
						item.SetToEndValue (transform);
					}
				}
			}
		}

		void AskPlay (string animName)
		{
			if (AnimGroupName == animName) {
				SetToBeginValue ();
				Play ();
			}
		}

		public static void PlayAnim (GameObject target, string animGroupName)
		{
			target.SendMessage ("AskPlay", animGroupName);
		}
	}
}