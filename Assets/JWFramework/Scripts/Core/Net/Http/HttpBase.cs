using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using JWFramework.Net.Http.Private;

namespace JWFramework.Net.Http
{
	public class HttpBase
	{
		private HttpKit coreKit;
		private List<SendData> sendDataQueue;
		private SendData lastSendData;
		private bool sendBlock;

		public HttpBase (string serverUrl, float timeOut = 30)
		{
			coreKit = new HttpKit (serverUrl, SessionCompleted, timeOut);
			sendDataQueue = new List<SendData> ();
			lastSendData = null;
			sendBlock = false;
		}

		private void SessionCompleted (bool success, byte[] msg, object state)
		{
			this.BeforeSessionCompleted (success);
			this.MainSessionCompleted (success, msg, state);
			this.AfterSessionCompleted (success);
		}

		protected virtual void BeforeSessionCompleted (bool success)
		{
		}

		protected virtual void MainSessionCompleted (bool success, byte[] msg, object state)
		{
		}

		protected virtual void AfterSessionCompleted (bool success)
		{
			UnlockSend ();
		}

		public void Send (byte[] data, object state)
		{
			if (sendDataQueue.Count <= 0 && !sendBlock && !coreKit.Sending) {
				this.BeforeMainSend ();
				this.MainSend (data, state);
				this.AfterMainSend ();
			} else {
				sendDataQueue.Add (new SendData (data, state));
			}
		}

		protected virtual void BeforeMainSend ()
		{
			BlockSend ();
		}

		private void MainSend (byte[] data, object state)
		{
			lastSendData = new SendData (data, state);
			coreKit.Send (data, state);
		}

		protected virtual void AfterMainSend ()
		{
		}

		protected void BlockSend ()
		{
			sendBlock = true;
		}

		protected void UnlockSend ()
		{
			sendBlock = false;
		}

		protected void ResendLastSendData ()
		{
			if (lastSendData != null) {
				sendDataQueue.Insert (0, lastSendData);
			}
		}

		public void Tick (float delta)
		{
			coreKit.Tick (delta);
			if (!sendBlock && !coreKit.Sending) {
				if (sendDataQueue.Count > 0) {
					var sendData = sendDataQueue [0];
					sendDataQueue.RemoveAt (0);
					this.BeforeMainSend ();
					this.MainSend (sendData.msg, sendData.state);
					this.AfterMainSend ();
				}
			}
		}

		private class SendData
		{
			public byte[] msg;
			public object state;

			public SendData (byte[] msg, object state)
			{
				this.msg = new byte[msg.Length];
				System.Buffer.BlockCopy (msg, 0, this.msg, 0, msg.Length);
				this.state = state;
			}
		}
	}
}