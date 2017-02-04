using UnityEngine;
using System.Collections;

namespace JWFramework
{
	public class ScenesManager
	{
		public static void LoadScene (string sceneName, UnityEngine.SceneManagement.LoadSceneMode mode)
		{
			UnityEngine.SceneManagement.SceneManager.LoadScene (sceneName, mode);
		}

		public static AsyncOperation LoadSceneAsync (string sceneName, UnityEngine.SceneManagement.LoadSceneMode mode)
		{
			return UnityEngine.SceneManagement.SceneManager.LoadSceneAsync (sceneName, mode);
		}

		public static void UnloadScene (string sceneName)
		{
			UnityEngine.SceneManagement.SceneManager.UnloadScene (sceneName);
		}

		public static void MoveGameObjectToScene (GameObject go, string sceneName)
		{
			UnityEngine.SceneManagement.Scene targetScene = UnityEngine.SceneManagement.SceneManager.GetSceneByName (sceneName);
			if (targetScene.IsValid ()) {
				UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene (go, targetScene);
			}
		}
	}
}