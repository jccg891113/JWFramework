using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace JWFramework.Net.Http.Private
{
	public delegate void HttpSessionCompleted (bool success,byte[] msg,object state);
	
	public class HttpKit
	{
		/// <summary>
		/// The server URL.
		/// </summary>
		private string serverUrl;
		/// <summary>
		/// The core "WWW" kit.
		/// </summary>
		private WWW coreKit;
		/// <summary>
		/// The delta time begin at send.
		/// </summary>
		private float sendDeltaTime;
		/// <summary>
		/// The send time limit.
		/// </summary>
		private float sendTimeOut;
		/// <summary>
		/// The receive message block.
		/// </summary>
		private bool receiveMsgBlock;
		/// <summary>
		/// The state of the send.
		/// </summary>
		private object sendState;
		/// <summary>
		/// The session completed delegate.
		/// </summary>
		private HttpSessionCompleted sessionCompleted;

		/// <summary>
		/// Initializes a new instance of the <see cref="JWFramework.Net.HttpKit"/> class.
		/// </summary>
		/// <param name="serverUrl">The Server URL.</param>
		/// <param name="sessionCompleted">Session completed delegate.</param>
		/// <param name="timeOut">Time out.(s)</param>
		public HttpKit (string serverUrl, HttpSessionCompleted sessionCompleted, float timeOut = 30)
		{
			this.serverUrl = serverUrl;
			this.coreKit = null;
			this.sendDeltaTime = 0;
			this.sendTimeOut = timeOut;
			this.receiveMsgBlock = false;
			this.sessionCompleted = sessionCompleted;
		}

		public bool Sending {
			get {
				return coreKit != null;
			}
		}

		/// <summary>
		/// Send the net message data.
		/// </summary>
		/// <param name="data">net message.</param>
		public bool Send (byte[] data, object state)
		{
			if (coreKit != null) {
				return false;
			}
			try {
				SendByWWW (data);
				receiveMsgBlock = true;
				sendState = state;
				return true;
			} catch (System.Exception we) {
				JWDebug.Log ("Send: " + we);
				return false;
			}
		}

		private void SendByWWW (byte[] data)
		{
			coreKit = new WWW (serverUrl, data);
			sendDeltaTime = 0;
		}

		/// <summary>
		/// Http Tick
		/// </summary>
		/// <param name="delta">Delta Time.(s)</param>
		public void Tick (float delta)
		{
			WWWTick (delta);
		}

		private void WWWTick (float delta)
		{
			if (coreKit != null) {
				sendDeltaTime += delta;
				if (coreKit.isDone) {
					if (!string.IsNullOrEmpty (coreKit.error)) {
						SessionCompleted (false, null);
						JWDebug.LogError ("UpdataCompleted error: " + coreKit.error);
					} else {
						if (coreKit.bytes != null && coreKit.bytes.Length > 0) {
							if (receiveMsgBlock) {
								receiveMsgBlock = false;
								SessionCompleted (true, coreKit.bytes);
							}
						}
					}
					coreKit = null;
				} else {
					if (sendDeltaTime > sendTimeOut) {
						SessionCompleted (false, null);
						JWDebug.LogError ("UpdataCompleted error: Custom Time Out");
						coreKit = null;
					}
				}
			}
		}

		private void SessionCompleted (bool success, byte[] msg)
		{
			if (sessionCompleted != null) {
				sessionCompleted (success, msg, sendState);
			}
		}
	}
}