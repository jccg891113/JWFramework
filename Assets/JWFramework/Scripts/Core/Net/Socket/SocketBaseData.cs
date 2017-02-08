using UnityEngine;
using System.Collections;

namespace JWFramework.Net.Socket.Private
{
	public class SocketBaseData
	{
		private int headLength;
		private int msgLengthInfoOffset;
		private int msgLengthInfoCount;
		
		private bool autoConnect = false;
		private float autoConnectDelay;

		#region P

		public int HeadLength{ get { return headLength; } }

		public int MsgLengthInfoOffset{ get { return msgLengthInfoOffset; } }

		public int MsgLengthInfoCount{ get { return msgLengthInfoCount; } }

		public bool AutoConnect{ get { return autoConnect; } }

		public float AutoConnectDelay{ get { return autoConnectDelay; } }

		public int MsgLengthInfoNeedCount {
			get {
				return msgLengthInfoOffset + msgLengthInfoCount;
			}
		}

		#endregion

		public SocketBaseData (int msgHeadLength, int msgLengthFlagOffset, int msgLengthFlagCount)
		{
			this.headLength = msgHeadLength;
			this.msgLengthInfoOffset = msgLengthFlagOffset;
			this.msgLengthInfoCount = msgLengthFlagCount;
			this.autoConnect = false;
		}

		public void SetAutoConnect (bool auto, float delay)
		{
			this.autoConnect = auto;
			this.autoConnectDelay = delay;
		}
	}
}