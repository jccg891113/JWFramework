using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using JWFramework.Audio.Private;

namespace JWFramework.Audio
{
	public class AudioBase
	{
		private AudioListener _listener;
		private AudioSource _backMusicSource;
		private AudioSource _audioSource;

		private AudioListener listener {
			get {
				if (_listener == null || !_listener.enabled) {
					var listeners = GameObject.FindObjectsOfType<AudioListener> ();
					foreach (var item in listeners) {
						if (item.enabled) {
							_listener = item;
							backMusicSource.transform.parent = listener.transform;
							backMusicSource.transform.localPosition = Vector3.zero;
							break;
						}
					}
				}
				return _listener;
			}
		}

		private AudioSource backMusicSource {
			get {
				if (_backMusicSource == null) {
					GameObject _audioListener = new GameObject ("_backMusicSource");
					_backMusicSource = _audioListener.AddComponent<AudioSource> ();
					_audioListener.transform.parent = listener.transform;
				}
				return _backMusicSource;
			}
		}

		private AudioSource audioSource {
			get {
				if (_audioSource == null) {
					GameObject _audioListener = new GameObject ("_audioSource");
					_audioSource = _audioListener.AddComponent<AudioSource> ();
					_audioListener.transform.parent = listener.transform;
				}
				return _audioSource;
			}
		}

		public bool bgmEnable {
			get {
				if (!PlayerPrefs.HasKey ("BGMEnable"))
					return true;
				return PlayerPrefs.GetInt ("BGMEnable", 1) == 1;
			}
			set { 
				if (value) {
					PlayerPrefs.SetInt ("BGMEnable", 1);
					backMusicSource.volume = 1;
				} else {
					PlayerPrefs.SetInt ("BGMEnable", 0);
					backMusicSource.volume = 0;
				}
			}
		}

		public bool audioEnable {
			get {
				if (!PlayerPrefs.HasKey ("AudioEnable"))
					return true;
				return PlayerPrefs.GetInt ("AudioEnable", 1) == 1;
			}
			set { 
				if (value) {
					PlayerPrefs.SetInt ("AudioEnable", 1);
					audioSource.volume = 1;
				} else {
					PlayerPrefs.SetInt ("AudioEnable", 0);
					audioSource.volume = 0;
				}
			}
		}

		public void PlayMusic (string name)
		{
			if (!bgmEnable) {
				return;
			}
			AudioClip clip = GetAudioclip (name);
			if (clip != null) {
				backMusicSource.clip = clip;
				backMusicSource.loop = true;
				backMusicSource.Play ();
			}
		}

		protected virtual AudioClip GetAudioclip (string name)
		{
			Object clipObj = Resources.Load (name);
			if (clipObj != null) {
				return (clipObj as AudioClip);
			} else {
				JWDebug.LogError ("Audio resource: " + name + " not find!");
				return null;
			}
		}

		public void StopMusic ()
		{
			backMusicSource.Stop ();
			if (backMusicSource.clip != null) {
				Resources.UnloadAsset (backMusicSource.clip);
				backMusicSource.clip = null;
			}
		}

		private Dictionary<string, AudioItem> _audioPlayData;

		private Dictionary<string, AudioItem> audioPlayData {
			get {
				if (_audioPlayData == null) {
					_audioPlayData = new Dictionary<string, AudioItem> ();
				}
				return _audioPlayData;
			}
		}

		public void PlayAudio (string name, bool allowMultiple = true)
		{
			if (!audioEnable) {
				return;
			}
			if (allowMultiple || !audioPlayData.ContainsKey (name) || !audioPlayData [name].isPlaying) {
				AudioClip clip = GetAudioclip (name);
				if (clip != null) {
					audioSource.PlayOneShot (clip);
					if (!allowMultiple) {
						audioPlayData [name] = new AudioItem (clip);
					}
				}
			}
		}

		public void PlayAudioOverScene (string name, bool allowMultiple = true)
		{
			if (!audioEnable) {
				return;
			}
			if (allowMultiple || !audioPlayData.ContainsKey (name) || !audioPlayData [name].isPlaying) {
				CleanAudioOverScene ();
				AudioClip clip = GetAudioclip (name);
				if (clip != null) {
					audioSource.clip = clip;
					audioSource.loop = false;
					audioSource.Play ();
					if (!allowMultiple) {
						audioPlayData [name] = new AudioItem (clip);
					}
				}
			}
		}

		public void CleanAudioOverScene ()
		{
			audioSource.Stop ();
			if (audioSource.clip != null) {
				Resources.UnloadAsset (audioSource.clip);
				audioSource.clip = null;
			}
		}
	}
}