using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System;
using System.Net;
using System.Net.Sockets;

namespace JWFramework.Net.Socket
{
	public class SocketKit
	{
		Thread sendThread;
		Thread receiveThread;
		
		private System.Net.Sockets.Socket socket;
		private AddressFamily addressFamily = AddressFamily.InterNetwork;
		private string address;
		private int port;
		
		private int msgHeadLength;
		private int msgLengthFlagOffset;
		private int msgLengthFlagCount;
		
		private Queue<object> msgQueue = new Queue<object> ();
		
		private Queue<NetData> sendDataQueue = new Queue<NetData> ();
		private Queue<NetData> receiveDataQueue = new Queue<NetData> ();
		public Queue<NetData> SpliteDataQueue = new Queue<NetData> ();

		public SocketKit (string address, int port, int msgHeadLength, int msgLengthFlagOffset, int msgLengthFlagCount)
		{
			this.address = address;
			this.port = port;
			this.msgQueue = new Queue<object> ();
			this.sendDataQueue = new Queue<NetData> ();
			this.receiveDataQueue = new Queue<NetData> ();
			this.SpliteDataQueue = new Queue<NetData> ();
			this.addressFamily = AddressFamily.InterNetwork;
			
			this.msgHeadLength = msgHeadLength;
			this.msgLengthFlagOffset = msgLengthFlagOffset;
			this.msgLengthFlagCount = msgLengthFlagCount;
			
			SetIPV6ForIOS ();
			
			socket = new System.Net.Sockets.Socket (addressFamily, SocketType.Stream, ProtocolType.Tcp);
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

		public void Tick ()
		{
			lock (msgQueue) {
				while (msgQueue.Count > 0) {
					object msg = msgQueue.Dequeue ();
					JWDebug.LogWarning (msg, JWDebug.LogType.net_socket);
				}
			}
			SplitNetData ();
		}

		private void SplitNetData ()
		{
			lock (receiveDataQueue) {
				byte[] allByte = { };
				// generate all byte data
				while (receiveDataQueue.Count > 0) {
					var receiveData = receiveDataQueue.Dequeue ();
					byte[] tmp = new byte[allByte.Length + receiveData.length];
					System.Buffer.BlockCopy (allByte, 0, tmp, 0, allByte.Length);
					System.Buffer.BlockCopy (receiveData.msg, 0, tmp, allByte.Length, receiveData.length);
				}
				// splite net data
				while (allByte.Length >= (msgLengthFlagOffset + msgLengthFlagCount)) {
					int length = ByteFunc.ByteToInt (allByte, msgLengthFlagOffset, msgLengthFlagCount);
					if (allByte.Length >= (msgHeadLength + length)) {
						byte[] completeData = new byte[msgHeadLength + length];
						System.Buffer.BlockCopy (allByte, 0, completeData, 0, (msgHeadLength + length));
						SpliteDataQueue.Enqueue (new NetData (completeData));
						
						if (allByte.Length - msgHeadLength - length > 0) {
							byte[] tmp = new byte[allByte.Length - msgHeadLength - length];
							System.Buffer.BlockCopy (allByte, (msgHeadLength + length), tmp, 0, (allByte.Length - msgHeadLength - length));
							allByte = new byte[tmp.Length];
							System.Buffer.BlockCopy (tmp, 0, allByte, 0, tmp.Length);
						} else {
							allByte = new byte[0];
						}
					}
				}
				// resave last no complete net data
				if (allByte.Length > 0) {
					receiveDataQueue.Enqueue (new NetData (allByte));
				}
			}
		}

		public void Connect ()
		{
			try {
				socket.BeginConnect (this.address, this.port, ConnectHandler, socket);
				socket.SendTimeout = 5000;
			} catch (Exception e) {
				lock (msgQueue) {
					msgQueue.Enqueue (e);
				}
			}
		}

		private void ConnectHandler (IAsyncResult async)
		{
			System.Net.Sockets.Socket s = (System.Net.Sockets.Socket)async.AsyncState;
			try {
				s.EndConnect (async);
				if (s.Connected) {
					lock (msgQueue) {
						msgQueue.Enqueue ("Success Connect: " + this.address + ":" + this.port);
					}
					ConnectSuccess ();
				} else {
					lock (msgQueue) {
						msgQueue.Enqueue ("Socket connect Error");
					}
				}
			} catch (Exception e) {
				lock (msgQueue) {
					msgQueue.Enqueue ("Connect error.\n" + e);
				}
				s.EndConnect (async);
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

		public void Send (byte[] msg)
		{
			lock (sendDataQueue) {
				sendDataQueue.Enqueue (new NetData (msg));
			}
		}

		private void SendThread ()
		{
			while (true) {
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
					} catch (Exception e) {
						lock (msgQueue) {
							msgQueue.Enqueue (e);
						}
					}
				}
			}
		}

		private void ReceiveThread ()
		{
			byte[] msgTmpPool = new byte[4096];
			while (true) {
				try {
					int read = socket.Receive (msgTmpPool);
					if (read <= 0) {
					} else {
						lock (receiveDataQueue) {
							receiveDataQueue.Enqueue (new NetData (read, msgTmpPool));
						}
					}
				} catch {
				}
			}
		}

		//		public bool SendMessage (byte[] msg, object msgInfo)
		//		{
		//			SendHelper sh = new SendHelper (socket, msg, msgInfo);
		//			socket.BeginSend (msg, 0, msg.Length, SocketFlags.None, SendHandler, sh);
		//			return true;
		//		}
		//
		//		private void SendHandler (IAsyncResult async)
		//		{
		//			SendHelper sh = (SendHelper)async.AsyncState;
		//			if (!sh.socket.Connected) {
		//				return;
		//			}
		//			try {
		//				int sendCount = sh.socket.EndSend (async);
		//				sh.sentNumber += sendCount;
		//				if (sh.sentNumber < sh.data.Length) {
		//					socket.BeginSend (sh.data, sh.sentNumber, sh.data.Length - sh.sentNumber, SocketFlags.None, SendHandler, sh);
		//				}
		//				lock (msgQueue) {
		//					msgQueue.Enqueue (string.Format ("---> Length:{0}, MsgInfo:{1}", sh.sentNumber, sh.msgInfo));
		//				}
		//			} catch (Exception e) {
		//				msgQueue.Enqueue ("End send error.\n" + e);
		//				sh.socket.EndSend (async);
		//			}
		//		}
		//
		//		internal class SendHelper
		//		{
		//			public System.Net.Sockets.Socket socket;
		//			public int sentNumber;
		//			public object msgInfo;
		//			public byte[] data;
		//
		//			public SendHelper (Socket s, byte[] msg, object msgInfo)
		//			{
		//				this.socket = s;
		//				this.sentNumber = 0;
		//				this.msgInfo = msgInfo;
		//				this.data = new byte[msg.Length];
		//				System.Buffer.BlockCopy (msg, 0, this.data, 0, msg.Length);
		//			}
		//		}

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