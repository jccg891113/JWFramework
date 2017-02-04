using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace JWFramework.Net.Socket
{
	public class SocketBase
	{
		private SocketKit coreKit;
		
		private bool autoConnect = false;

		public SocketBase (string address, int port, int msgHeadLength, int msgLengthFlagOffset = 0, int msgLengthFlagCount = 4)
		{
			coreKit = new SocketKit (address, port, msgHeadLength, msgLengthFlagOffset, msgLengthFlagCount);
		}

		public void Connect ()
		{
			coreKit.Connect ();
		}

		public void Send (byte[] msg)
		{
			coreKit.Send (msg);
		}

		public void Tick ()
		{
			coreKit.Tick ();
			while (coreKit.SpliteDataQueue.Count > 0) {
				MainSessionCompleted (coreKit.SpliteDataQueue.Dequeue ().msg);
			}
		}

		protected virtual void MainSessionCompleted (byte[] msg)
		{
		}
	}
}