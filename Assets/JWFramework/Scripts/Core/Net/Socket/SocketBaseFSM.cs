using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Net;
using System.Net.Sockets;
using JWFramework.FSM;

namespace JWFramework.Net.Socket.Base
{
	public class SocketBaseFSM : FSMachine<SocketState>
	{
		public SocketBase socketBase{ get; private set; }

		public float ConnectTime {
			get {
				return this.GetState<SocketConnecting> (SocketState.CONNECTING).connectTime;
			}
		}

		public SocketBaseFSM (SocketBase socketBase)
		{
			this.socketBase = socketBase;
			AddState (new SocketCreate (this));
			AddState (new SocketConnecting (this));
			AddState (new SocketWorking (this));
			AddState (new SocketError (this));
			AddState (new SocketClose (this));
		}
	}

	public class SocketStateBase : FState<SocketState>
	{
		protected SocketBaseFSM ctrl;

		public SocketStateBase (SocketState socketState, SocketBaseFSM ctrl) : base (socketState, ctrl)
		{
			this.ctrl = ctrl;
		}
	}

	public class SocketCreate : SocketStateBase
	{
		public SocketCreate (SocketBaseFSM ctrl) : base (SocketState.CREATE, ctrl)
		{
		}
	}

	public class SocketConnecting : SocketStateBase
	{
		public float connectTime;

		public SocketConnecting (SocketBaseFSM ctrl) : base (SocketState.CONNECTING, ctrl)
		{
			connectTime = 0;
		}

		public override void Enter (SocketState beforeStateType, JWData enterParamData)
		{
			connectTime = 0;
			base.Enter (beforeStateType, enterParamData);
		}

		public override void Tick (float delta)
		{
			connectTime += delta;
		}
	}

	public class SocketWorking : SocketStateBase
	{
		public SocketWorking (SocketBaseFSM ctrl) : base (SocketState.WORKING, ctrl)
		{
		}
	}

	public class SocketError : SocketStateBase
	{
		private float reconnectTime;

		public SocketError (SocketBaseFSM ctrl) : base (SocketState.ERROR_CLOSE, ctrl)
		{
			reconnectTime = 0;
		}

		public override void Enter (SocketState beforeStateType, JWData enterParamData)
		{
			if (!ctrl.socketBase.baseData.AutoConnect) {
				ctrl.socketBase.AskReconnectHandler ();
			}
		}

		public override void Tick (float delta)
		{
			reconnectTime += delta;
			if (reconnectTime > ctrl.socketBase.baseData.AutoConnectDelay) {
				if (ctrl.socketBase.baseData.AutoConnect) {
					ctrl.socketBase.ReconnectMain ();
				}
				reconnectTime = 0;
			}
		}
	}

	public class SocketClose : SocketStateBase
	{
		public SocketClose (SocketBaseFSM ctrl) : base (SocketState.CLOSE, ctrl)
		{
		}
	}
}