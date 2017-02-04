using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System;
using System.Net;
using System.Net.Sockets;

namespace JWFramework.Net.Socket
{
	public delegate void SocketVoidDelegate ();
	public delegate void SocketStateChange (SocketState newState);
	
	public enum SocketState : int
	{
		CREATE,
		CONNECTING,
		WORKING,
		CLOSE,
		ERROR_CLOSE,
	}

	public enum SocketErrorType : int
	{
		NONE,
		CONNECT_ERROR,
		RECEIVE_ERROR,
		SEND_ERROR,
		ERROR,
		DISCONNECT,
	}

	public class SocketKit
	{
		Thread sendThread;
		Thread receiveThread;

		public SocketState state{ get; private set; }

		public SocketErrorType error{ get; private set; }

		public bool connected{ get; private set; }

		private System.Net.Sockets.Socket socket;
		
		private string address;
		private int port;
		
		private Queue<object> msgQueue = new Queue<object> ();
		
		private AddressFamily addressFamily = AddressFamily.InterNetwork;
		
		private Queue<NetData> sendDataQueue = new Queue<NetData> ();
		private Queue<NetData> receiveDataQueue = new Queue<NetData> ();

		private SocketVoidDelegate afterConnectDelegate;
		private SocketStateChange socketStateChange;

		public SocketKit (string ipv4Address, int port, SocketStateChange socketStateChange)
		{
			this.address = ipv4Address;
			this.addressFamily = AddressFamily.InterNetwork;
			this.port = port;
			this.msgQueue = new Queue<object> ();
			this.socketStateChange = socketStateChange;
			
			SetIPV6ForIOS ();
			
			ResetInit ();
		}

		private void SetIPV6ForIOS ()
		{
			#if UNITY_IOS
			AddressFamily newAddressFamily = AddressFamily.InterNetwork;
			string newServerIp = "";
			IOSIPV6.GetIPType (address, port.ToString (), out newServerIp, out newAddressFamily);
			if (!string.IsNullOrEmpty (newServerIp)) {
				this.address = newServerIp;
				this.addressFamily = newAddressFamily;
			}
			Debug.Log ("Socket AddressFamily :" + addressFamily.ToString () + "ServerIp:" + newServerIp);
			#endif
		}

		public void ResetInit ()
		{
			this.sendDataQueue = new Queue<NetData> ();
			this.receiveDataQueue = new Queue<NetData> ();
			socket = new System.Net.Sockets.Socket (addressFamily, SocketType.Stream, ProtocolType.Tcp);
			
			SetSocketState (false, SocketState.CREATE, SocketErrorType.NONE);
		}

		private void SetSocketState (bool connected, SocketState currState, SocketErrorType currError)
		{
			this.connected = connected;
			this.state = currState;
			this.error = currError;
			lock (this.socketStateChange) {
				if (this.socketStateChange != null) {
					this.socketStateChange (currState);
				}
			}
		}

		public void Tick ()
		{
			lock (msgQueue) {
				while (msgQueue.Count > 0) {
					object msg = msgQueue.Dequeue ();
					JWDebug.LogWarning (msg, JWDebug.LogType.net_socket);
				}
			}
		}

		public void Connect (SocketVoidDelegate afterConnectDelegate)
		{
			this.afterConnectDelegate = afterConnectDelegate;
			try {
				socket.BeginConnect (this.address, this.port, ConnectHandler, socket);
				socket.SendTimeout = 5000;
				SetSocketState (false, SocketState.CONNECTING, SocketErrorType.NONE);
			} catch (Exception e) {
				EnqueueLog (e);
				CloseBase ();
				SetSocketState (false, SocketState.ERROR_CLOSE, SocketErrorType.CONNECT_ERROR);
			}
		}

		private void EnqueueLog (object log)
		{
			lock (msgQueue) {
				msgQueue.Enqueue (log);
			}
		}

		private void ConnectHandler (IAsyncResult async)
		{
			System.Net.Sockets.Socket s = (System.Net.Sockets.Socket)async.AsyncState;
			try {
				s.EndConnect (async);
				if (s.Connected) {
					EnqueueLog ("Success Connect: " + this.address + ":" + this.port);
					SetSocketState (true, SocketState.WORKING, SocketErrorType.NONE);
					ConnectSuccess ();
					AfterConnectSuccess ();
				} else {
					EnqueueLog ("Socket connect Error");
					CloseBase ();
					SetSocketState (false, SocketState.ERROR_CLOSE, SocketErrorType.CONNECT_ERROR);
				}
			} catch (Exception e) {
				s.EndConnect (async);
				EnqueueLog ("Connect error.\n" + e);
				CloseBase ();
				SetSocketState (false, SocketState.ERROR_CLOSE, SocketErrorType.CONNECT_ERROR);
			}
		}

		private void ConnectSuccess ()
		{
			sendThread = new Thread (new ThreadStart (SendThread));
			sendThread.IsBackground = true;
			sendThread.Start ();
			
			receiveThread = new Thread (new ThreadStart (ReceiveThread));
			receiveThread.IsBackground = true;
			receiveThread.Start ();
		}

		private void AfterConnectSuccess ()
		{
			if (this.afterConnectDelegate != null) {
				this.afterConnectDelegate ();
			}
		}

		public void Send (byte[] msg)
		{
			lock (sendDataQueue) {
				sendDataQueue.Enqueue (new NetData (msg));
			}
		}

		private void SendThread ()
		{
			while (connected) {
				NetData willSendData = null;
				lock (sendDataQueue) {
					if (sendDataQueue.Count > 0) {
						willSendData = sendDataQueue.Dequeue ();
					}
				}
				if (willSendData != null) {
					int hadSend = 0;
					int send = 0;
					try {
						while (hadSend + send < willSendData.length) {
							hadSend += send;
							send = socket.Send (willSendData.msg, hadSend, willSendData.length - hadSend, SocketFlags.None);
						}
					} catch (ThreadAbortException) {
						break;
					} catch (Exception e) {
						EnqueueLog (e);
						CloseBase ();
						SetSocketState (false, SocketState.ERROR_CLOSE, SocketErrorType.SEND_ERROR);
					}
				}
			}
		}

		private void ReceiveThread ()
		{
			byte[] msgTmpPool = new byte[4096];
			while (connected) {
				try {
					int read = socket.Receive (msgTmpPool);
					if (read <= 0) {
						CloseBase ();
						SetSocketState (false, SocketState.CLOSE, SocketErrorType.NONE);
					} else {
						lock (receiveDataQueue) {
							receiveDataQueue.Enqueue (new NetData (read, msgTmpPool));
						}
					}
				} catch (ThreadAbortException) {
					break;
				} catch (Exception e) {
					EnqueueLog (e);
					CloseBase ();
					SetSocketState (false, SocketState.ERROR_CLOSE, SocketErrorType.RECEIVE_ERROR);
				}
			}
		}

		public void Close ()
		{
			CloseBase ();
			SetSocketState (false, SocketState.CLOSE, SocketErrorType.NONE);
		}

		private void CloseBase ()
		{
			lock (socket) {
				if (socket != null) {
					if (socket.Connected) {
						socket.Close ();
					}
				}
			}
			lock (sendThread) {
				if (sendThread != null) {
					sendThread.Abort ();
					sendThread = null;
				}
			}
			lock (receiveThread) {
				if (receiveThread != null) {
					receiveThread.Abort ();
					receiveThread = null;
				}
			}
		}

		public void SpeClose (SocketState closeState, SocketErrorType closeError)
		{
			CloseBase ();
			SetSocketState (false, closeState, closeError);
		}

		public byte[] GetReceivedBytes ()
		{
			lock (receiveDataQueue) {
				byte[] res = new byte[0];
				while (receiveDataQueue.Count > 0) {
					var receiveData = receiveDataQueue.Dequeue ();
					byte[] tmp = new byte[res.Length + receiveData.length];
					System.Buffer.BlockCopy (res, 0, tmp, 0, res.Length);
					System.Buffer.BlockCopy (receiveData.msg, 0, tmp, res.Length, receiveData.length);
					res = new byte[tmp.Length];
					System.Buffer.BlockCopy (tmp, 0, res, 0, tmp.Length);
				}
				return res;
			}
		}

		public class NetData
		{
			public byte[] msg;

			public int length {
				get {
					return msg.Length;
				}
			}

			public NetData (byte[] msg)
			{
				this.msg = new byte[msg.Length];
				System.Buffer.BlockCopy (msg, 0, this.msg, 0, msg.Length);
			}

			public NetData (int length, byte[] msg)
			{
				this.msg = new byte[length];
				System.Buffer.BlockCopy (msg, 0, this.msg, 0, length);
			}
		}
	}
}