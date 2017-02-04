using System.Runtime.InteropServices;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class IOSIPV6
{
	#if UNITY_IOS

	[DllImport ("__Internal")]
	private static extern string getIPv6 (string mHost, string mPort);

	private static string GetIPv6 (string mHost, string mPort)
	{
		#if UNITY_IOS && !UNITY_EDITOR
		string mIPv6 = getIPv6(mHost, mPort);
		return mIPv6;
		#else
		return mHost + "&&ipv4";
		#endif
	}

	public static void GetIPType (string serverIp, string serverPorts, out string newServerIp, out System.Net.Sockets.AddressFamily mIPType)
	{
		newServerIp = serverIp;
		mIPType = System.Net.Sockets.AddressFamily.InterNetwork;
		try {
			string mIPv6 = GetIPv6 (serverIp, serverPorts);
			if (!string.IsNullOrEmpty (mIPv6)) {
				string[] m_StrTemp = System.Text.RegularExpressions.Regex.Split (mIPv6, "&&");
				if (m_StrTemp != null && m_StrTemp.Length >= 2) {
					string IPType = m_StrTemp [1];
					if (IPType == "ipv6") {
						newServerIp = m_StrTemp [0];
						mIPType = System.Net.Sockets.AddressFamily.InterNetworkV6;
					}
				}
			}
		} catch (System.Exception e) {
			Debug.Log ("GetIPv6 error:" + e);
		}
	}

	#endif
}