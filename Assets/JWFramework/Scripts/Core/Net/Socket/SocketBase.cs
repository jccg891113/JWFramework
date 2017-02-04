using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace JWFramework.Net.Socket
{
	public class SocketBase
	{
		private SocketKit coreKit;
		
		private int msgHeadLength;
		private int coreMsgLengthFlagOffset;
		private int coreMsgLengthFlagCount;

		public bool autoConnect = false;
		private SocketVoidDelegate afterConnectDelegate;
		private SocketVoidDelegate reconnectHandler;
		
		public float connectTime;
		
		private Queue<SocketKit.NetData> receivedMsgData;
		private byte[] tmpReceivedMsg;
		
		private SocketBaseFSM logicFSM;

		public SocketBase (string address, int port, int msgHeadLength, int msgLengthFlagOffset = 0, int msgLengthFlagCount = 4)
		{
			coreKit = new SocketKit (address, port, new JWFramework.Net.Socket.SocketStateChange (SocketStateChange));
			this.msgHeadLength = msgHeadLength;
			this.coreMsgLengthFlagOffset = msgLengthFlagOffset;
			this.coreMsgLengthFlagCount = msgLengthFlagCount;
			receivedMsgData = new Queue<SocketKit.NetData> ();
			tmpReceivedMsg = new byte[0];
			
			logicFSM = new SocketBaseFSM (this);
		}

		private void SocketStateChange (SocketState newState)
		{
			logicFSM.Goto (newState, null);
		}

		public void Connect (bool autoConnect, SocketVoidDelegate afterConnectDelegate, SocketVoidDelegate reconnectHandler)
		{
			this.autoConnect = autoConnect;
			this.afterConnectDelegate = afterConnectDelegate;
			this.reconnectHandler = reconnectHandler;
			_Connect ();
		}

		private void _Connect ()
		{
			SpliteNetData ();
			tmpReceivedMsg = new byte[0];
			coreKit.ResetInit ();
			coreKit.Connect (this.afterConnectDelegate);
		}

		public void ReconnectMain ()
		{
			_Connect ();
		}

		/// <summary>
		/// private
		/// </summary>
		public void NoAutoReconnect ()
		{
			this.reconnectHandler ();
		}

		public void Send (byte[] msg)
		{
			coreKit.Send (msg);
		}

		public void Tick (float delta)
		{
			coreKit.Tick ();
			logicFSM.Tick (delta);
			SpliteNetData ();
		}

		private void SpliteNetData ()
		{
			byte[] lastReceivedMsg = coreKit.GetReceivedBytes ();
			if (lastReceivedMsg.Length <= 0) {
				return;
			}
			byte[] allByte = new byte[lastReceivedMsg.Length + tmpReceivedMsg.Length];
			System.Buffer.BlockCopy (tmpReceivedMsg, 0, allByte, 0, tmpReceivedMsg.Length);
			System.Buffer.BlockCopy (lastReceivedMsg, 0, allByte, tmpReceivedMsg.Length, lastReceivedMsg.Length);
			// splite net data
			while (allByte.Length >= (coreMsgLengthFlagOffset + coreMsgLengthFlagCount)) {
				int msgLength = ByteFunc.ByteToInt (allByte, coreMsgLengthFlagOffset, coreMsgLengthFlagCount);
				int msgLengthHadHead = msgHeadLength + msgLength;
				if (allByte.Length >= msgLengthHadHead) {
					byte[] completeData = new byte[msgLengthHadHead];
					System.Buffer.BlockCopy (allByte, 0, completeData, 0, msgLengthHadHead);
					receivedMsgData.Enqueue (new SocketKit.NetData (completeData));

					if (allByte.Length - msgLengthHadHead > 0) {
						byte[] tmp = new byte[allByte.Length - msgLengthHadHead];
						System.Buffer.BlockCopy (allByte, msgLengthHadHead, tmp, 0, (allByte.Length - msgLengthHadHead));
						allByte = new byte[tmp.Length];
						System.Buffer.BlockCopy (tmp, 0, allByte, 0, tmp.Length);
					} else {
						allByte = new byte[0];
					}
				}
			}
			tmpReceivedMsg = new byte[allByte.Length];
			System.Buffer.BlockCopy (allByte, 0, tmpReceivedMsg, 0, allByte.Length);
		}
	}
}