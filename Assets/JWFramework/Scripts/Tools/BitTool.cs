using System;
using System.Collections;
using System.Collections.Generic;

namespace JWFramework.Tools
{
	public static class BitTool
	{
		/// <summary>
		/// Check the bit value is "1" in the specified state bit.
		/// </summary>
		/// <returns><c>true</c> if is "1" in the specified state bit; otherwise, <c>false</c>.</returns>
		/// <param name="state">State value.</param>
		/// <param name="bit">Bit num.</param>
		public static bool IsBit (this Int64 state, int bit)
		{
			long temp = state;
			return (temp >> (bit - 1) & 1) > 0;
		}

		#region Int

		/// <summary>
		/// Set value of the specified state bit is "1"
		/// </summary>
		/// <returns>Final State Value.</returns>
		/// <param name="baseState">Base state.</param>
		/// <param name="bit">Bit num.</param>
		public static int BitOn (this Int32 baseState, int bit)
		{
			int bitState = 1 << (bit - 1);
			return (bitState | baseState);
		}

		/// <summary>
		/// Set value of the specified state bit is "0"
		/// </summary>
		/// <returns>Final State Value.</returns>
		/// <param name="baseState">Base state.</param>
		/// <param name="bit">Bit num.</param>
		public static int BitOff (this Int32 baseState, int bit)
		{
			int bitReverseState = ~(1 << (bit - 1));
			return (bitReverseState & baseState);
		}

		#endregion

		#region Long

		/// <summary>
		/// Set value of the specified state bit is "1"
		/// </summary>
		/// <returns>Final State Value.</returns>
		/// <param name="baseState">Base state.</param>
		/// <param name="bit">Bit num.</param>
		public static long BitOn (this Int64 baseState, int bit)
		{
			long bitState = 1 << (bit - 1);
			return (bitState | baseState);
		}

		/// <summary>
		/// Set value of the specified state bit is "0"
		/// </summary>
		/// <returns>Final State Value.</returns>
		/// <param name="baseState">Base state.</param>
		/// <param name="bit">Bit num.</param>
		public static long BitOff (this Int64 baseState, int bit)
		{
			long bitReverseState = ~(1 << (bit - 1));
			return (bitReverseState & baseState);
		}

		#endregion
	}
}