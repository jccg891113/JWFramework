using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JWFramework.Tools
{
	public class DateTool
	{
		public static int serverTimeZone = 8;
		public const long milliSecondPerHour = 3600000;
		private static DateTime begin1970Time = new DateTime (1970, 1, 1);

		private static DateTime baseServerUtcTime;
		private static DateTime baseServerTime;
		private static DateTime baseClientTime;

		/// <summary>
		/// 获取客户端时间
		/// </summary>
		/// <returns>客户端时间</returns>
		public static DateTime clientCurrTime { get { return System.DateTime.Now; } }

		/// <summary>
		/// 获取客户端记录的服务器当前时区时间
		/// </summary>
		/// <returns>服务器当前时区时间</returns>
		public static DateTime serverCurrTime { get { return new DateTime (baseServerTime.Ticks).AddMilliseconds ((System.DateTime.Now - baseClientTime).TotalMilliseconds); } }

		/// <summary>
		/// 初始化基本服务器世界标准时间，基本客户端时间，服务器时区与服务器时间
		/// </summary>
		/// <param name="serverMilliSeconds">Server milli seconds.</param>
		/// <param name="serverTimeZone">Server time zone.</param>
		public static void Init (long serverMilliSeconds, int serverTimeZone)
		{
			DateTool.baseServerUtcTime = TransServerUtcTime (serverMilliSeconds);
			DateTool.baseClientTime = DateTime.Now;
			DateTool.serverTimeZone = serverTimeZone;
			DateTool.baseServerTime = baseServerUtcTime.AddHours (serverTimeZone);
		}

		public static void RefreshTimeBaseData (long serverMilliSeconds)
		{
			DateTool.baseServerUtcTime = TransServerUtcTime (serverMilliSeconds);
			DateTool.baseClientTime = DateTime.Now;
			DateTool.baseServerTime = baseServerUtcTime.AddHours (serverTimeZone);
		}

		public static long GetServerTime (long milliSeconds)
		{
			return milliSeconds + serverTimeZone * milliSecondPerHour;
		}

		public static long GetServerNowSeconds ()
		{
			return (long)Math.Floor ((serverCurrTime - begin1970Time).TotalSeconds);
		}

		public static long GetServerNowMilliSeconds ()
		{
			return (long)Math.Floor ((serverCurrTime - begin1970Time).TotalMilliseconds);
		}

		/// <summary>
		/// 服务器毫秒时间转换为服务器世界标准时间
		/// </summary>
		/// <returns>服务器世界标准时间</returns>
		/// <param name="serverMilliSeconds">服务器时间（自1970-1-1始毫秒值）</param>
		public static DateTime TransServerUtcTime (long serverMilliSeconds)
		{
			return (new DateTime (1970, 1, 1)).AddMilliseconds (serverMilliSeconds);
		}

		/// <summary>
		/// 服务器毫秒时间转换为服务器当前时区时间
		/// </summary>
		/// <returns>服务器当前时区时间</returns>
		/// <param name="serverMilliSeconds">服务器时间（自1970-1-1始毫秒值）</param>
		public static DateTime TransServerTime (long serverMilliSeconds)
		{
			return TransServerUtcTime (serverMilliSeconds).AddHours (serverTimeZone);
		}

		/// <summary>
		/// 服务器毫秒时间转换为客户端时间
		/// </summary>
		/// <returns>客户端时间</returns>
		/// <param name="serverMilliSeconds">服务器时间（自1970-1-1始毫秒值）</param>
		public static DateTime TransClientTime (long serverMilliSeconds)
		{
			DateTime serverTime = TransServerUtcTime (serverMilliSeconds);
			TimeSpan serverDelta = serverTime - baseServerUtcTime;
			return new DateTime (baseClientTime.Ticks).AddMilliseconds (serverDelta.TotalMilliseconds);
		}

		/// <summary>
		/// 判断两个日期是否是同一天
		/// </summary>
		/// <param name="date1"></param>
		/// <param name="date2"></param>
		/// <returns></returns>
		public static bool IsSameDay (DateTime date1, DateTime date2)
		{
			return ((date1.Year == date2.Year) && (date1.DayOfYear == date2.DayOfYear));
		}
	}
}
