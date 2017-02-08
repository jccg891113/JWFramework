using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using JWFramework.Net.Socket.Private;

namespace JWFramework.Net.Socket
{
	public class SocketBase
	{
		private SocketKit coreKit;
		
		public SocketBaseData baseData;
		private SocketBaseSpliteMsgTool receiveMsgTool;
		
		private SocketBaseFSM logicFSM;
		
		private SocketVoidDelegate afterConnectDelegate;
		private SocketVoidDelegate reconnectHandler;
		private SocketSocketStateDelegate socketStateTrigger;

		#region

		public float connectTime{ get { return logicFSM.ConnectTime; } }

		public bool Connected { get { return coreKit.connected; } }

		public SocketState CurrState { get { return logicFSM.CurrrentStateType; } }

		#endregion

		public SocketBase (string address, int port, SocketSocketStateDelegate socketStateTrigger, int msgHeadLength, int msgLengthFlagOffset = 0, int msgLengthFlagCount = 4)
		{
			this.logicFSM = new SocketBaseFSM (this);
			this.baseData = new SocketBaseData (msgHeadLength, msgLengthFlagOffset, msgLengthFlagCount);
			this.receiveMsgTool = new SocketBaseSpliteMsgTool ();
			this.socketStateTrigger = socketStateTrigger;
			
			this.coreKit = new SocketKit (address, port, SocketStateChange);
		}

		private void SocketStateChange (SocketState newState)
		{
			logicFSM.Goto (newState, null);
			if (this.socketStateTrigger != null) {
				this.socketStateTrigger (newState);
			}
		}

		public void Connect (bool autoConnect, SocketVoidDelegate afterConnectDelegate, SocketVoidDelegate reconnectHandler)
		{
			baseData.SetAutoConnect (autoConnect, 10);
			this.afterConnectDelegate = afterConnectDelegate;
			this.reconnectHandler = reconnectHandler;
			_Connect ();
		}

		private void _Connect ()
		{
			SpliteNetData ();
			receiveMsgTool.Clean ();
			coreKit.ResetInit ();
			coreKit.Connect (this.afterConnectDelegate);
		}

		private void SpliteNetData ()
		{
			byte[] lastReceivedMsg;
			if (coreKit.GetReceivedBytes (out lastReceivedMsg)) {
				receiveMsgTool.InsertBytes (lastReceivedMsg, baseData);
			}
		}

		public void ReconnectMain ()
		{
			_Connect ();
		}

		public void AskReconnectHandler ()
		{
			if (this.reconnectHandler != null) {
				this.reconnectHandler ();
			}
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

		public bool DequeueMsgData (out byte[] res)
		{
			return receiveMsgTool.GetMsgData (out res);
		}

		public void TimeOut ()
		{
			coreKit.SpeClose (SocketState.ERROR_CLOSE, SocketErrorType.ERROR);
		}

		public void Close ()
		{
			coreKit.Close ();
		}
	}
}