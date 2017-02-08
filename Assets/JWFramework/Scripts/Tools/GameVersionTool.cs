using UnityEngine;
using System.Collections;
using System.IO;

namespace JWFramework.Tools
{
	public class VersionTool
	{
		private static string versionFilePath = Application.temporaryCachePath + "/version";

		public static string GetSavedVersion (string defaultValue)
		{
			if (File.Exists (versionFilePath)) {
				string[] res = File.ReadAllLines (versionFilePath);
				if (res.Length > 0) {
					return res [0];
				} else {
					return defaultValue;
				}
			} else {
				return defaultValue;
			}
		}

		public static void SaveGameVersion (string newGameVersion)
		{
			if (!string.IsNullOrEmpty (newGameVersion)) {
				DirectoryInfo di = new DirectoryInfo (Path.GetDirectoryName (versionFilePath));
				if (!di.Exists) {
					di.Create ();
				}
				File.WriteAllText (versionFilePath, newGameVersion);
			} else {
				JWDebug.LogError ("New game version is NAN");
			}
		}

		public static int GetNewerVersion (int version1, int version2)
		{
			return System.Math.Max (version1, version2);
		}

		public static int GetVersionNum (int versionStrIndex, string version)
		{
			string[] versionNums = version.Split (new char[]{ '.' }, System.StringSplitOptions.RemoveEmptyEntries);
			try {
				string valueStr = versionNums [versionStrIndex];
				return int.Parse (valueStr);
			} catch (System.Exception e) {
				JWDebug.LogError (e);
				return -1;
			}
		}
	}
}