using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace JWFramework.Net.Socket.Base
{
	public class SocketBaseSpliteMsgTool
	{
		private Queue<SocketKit.NetData> receivedMsgData;
		private byte[] tmpReceivedMsg;

		public SocketBaseSpliteMsgTool ()
		{
			receivedMsgData = new Queue<SocketKit.NetData> ();
			tmpReceivedMsg = new byte[0];
		}

		public void InsertBytes (byte[] insertData, SocketBaseData baseData)
		{
			byte[] allByte = new byte[insertData.Length + tmpReceivedMsg.Length];
			System.Buffer.BlockCopy (tmpReceivedMsg, 0, allByte, 0, tmpReceivedMsg.Length);
			System.Buffer.BlockCopy (insertData, 0, allByte, tmpReceivedMsg.Length, insertData.Length);
			// splite net data
			while (allByte.Length >= baseData.MsgLengthInfoNeedCount) {
				int msgLength = ByteFunc.ByteToInt (allByte, baseData.MsgLengthInfoOffset, baseData.MsgLengthInfoCount);
				int msgLengthHadHead = baseData.HeadLength + msgLength;
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

		public bool GetMsgData (out byte[] res)
		{
			if (receivedMsgData.Count > 0) {
				var data = receivedMsgData.Dequeue ();
				res = data.msg;
				return true;
			}
			res = new byte[0];
			return false;
		}

		public void Clean ()
		{
			tmpReceivedMsg = new byte[0];
		}

		public void CleanAll ()
		{
			receivedMsgData = new Queue<SocketKit.NetData> ();
			tmpReceivedMsg = new byte[0];
		}
	}
}