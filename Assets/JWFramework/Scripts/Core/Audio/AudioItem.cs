using UnityEngine;
using System.Collections;

namespace JWFramework.Audio.Private
{
	public class AudioItem
	{
		float startTime;
		float audioLength;

		public AudioItem (AudioClip clip)
		{
			startTime = Time.time;
			audioLength = clip.length;
		}

		public bool isPlaying {
			get {
				return ((Time.time - startTime) < audioLength);
			}
		}
	}
}