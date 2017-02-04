using UnityEngine;
using System;
using System.Net;
using System.Collections;

namespace JWFramework.Net
{
	public class ByteFunc
	{
		#region To Byte[]

		public static byte[] ShortToByte (short s)
		{
			short destValue = IPAddress.HostToNetworkOrder (s);
			byte[] arrByte = BitConverter.GetBytes (destValue);
			//java与C#大小端不一，因此需进行反转
			return arrByte;
		}

		public static byte[] IntToByte (int n)
		{
			int destValue = IPAddress.HostToNetworkOrder (n);
			byte[] arrByte = BitConverter.GetBytes (destValue);
			return arrByte;
		}

		public static byte[] LongToByte (long l)
		{
			long destValue = IPAddress.HostToNetworkOrder (l);
			byte[] arrByte = BitConverter.GetBytes (destValue);
			return arrByte;
		}

		public static byte[] StringToByte (string s, int length = 0)
		{
			byte[] arrByte = System.Text.Encoding.ASCII.GetBytes (s);
			if (length == 0) {
				return arrByte;
			} else {
				byte[] result = new byte[length];
				Buffer.BlockCopy (arrByte, 0, result, 0, Math.Min (arrByte.Length, length));
				return result;
			}
		}

		#endregion

		#region Byte[] To

		public static short ByteToShort (byte[] msg, int nOffset)
		{
			byte[] arrByte = new byte[2];
			Buffer.BlockCopy (msg, nOffset, arrByte, 0, 2);
			short num = BitConverter.ToInt16 (arrByte, 0);
			return IPAddress.NetworkToHostOrder (num);
		}

		public static int ByteToInt (byte[] msg, int nOffset)
		{
			byte[] arrByte = new byte[4];
			Buffer.BlockCopy (msg, nOffset, arrByte, 0, 4);
			int num = BitConverter.ToInt32 (arrByte, 0);
			return IPAddress.NetworkToHostOrder (num);
		}

		public static int ByteToInt (byte[] msg, int nOffset, int nCount)
		{
			byte[] arrByte = new byte[4];
			Buffer.BlockCopy (msg, nOffset, arrByte, 0, nCount);
			int num = BitConverter.ToInt32 (arrByte, 0);
			return IPAddress.NetworkToHostOrder (num);
		}

		public static long ByteToLong (byte[] msg, int nOffset)
		{
			byte[] arrByte = new byte[8];
			Buffer.BlockCopy (msg, nOffset, arrByte, 0, 8);
			long num = BitConverter.ToInt64 (arrByte, 0);
			return IPAddress.NetworkToHostOrder (num);
		}

		#endregion
	}
}