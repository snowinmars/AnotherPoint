using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using log4net.Config;
using log4net.Core;

namespace AnotherPoint.Common
{
	public static class Log
	{
		private static ILog log = LogManager.GetLogger("LOGGER");

		public static ILogger Logger
		{
			get { return Log.log.Logger; }
		}

		private static int deep;

		private static string GetSpaces
		{
			get
			{
				StringBuilder sb = new StringBuilder();
				sb.Append(' ', deep);
				return sb.ToString();
			}
		}
		public static void Debug(object message)
		{
			Log.deep++;
			Log.log.Debug(GetSpaces + message);
		}

		public static void Debug(object message, Exception exception)
		{
			Log.deep++;
			Log.log.Debug(GetSpaces + message, exception);
		}

		public static void DebugFormat(string format, params object[] args)
		{
			Log.log.DebugFormat(format, args);
		}

		public static void DebugFormat(string format, object arg0)
		{
			Log.log.DebugFormat(format, arg0);
		}

		public static void DebugFormat(string format, object arg0, object arg1)
		{
			Log.log.DebugFormat(format, arg0, arg1);
		}

		public static void DebugFormat(string format, object arg0, object arg1, object arg2)
		{
			Log.log.DebugFormat(format, arg0, arg1, arg2);
		}

		public static void DebugFormat(IFormatProvider provider, string format, params object[] args)
		{
			Log.log.DebugFormat(provider, format, args);
		}

		public static void Info(object message)
		{
			Log.deep++;
			Log.log.Info(GetSpaces + message);
		}

		public static void Info(object message, Exception exception)
		{ 
			Log.deep++;
			Log.log.Info(GetSpaces + message, exception);
		}

		public static void InfoFormat(string format, params object[] args)
		{
			Log.log.InfoFormat(format, args);
		}

		public static void InfoFormat(string format, object arg0)
		{
			Log.log.InfoFormat(format, arg0);
		}

		public static void InfoFormat(string format, object arg0, object arg1)
		{
			Log.log.InfoFormat(format, arg0, arg1);
		}

		public static void InfoFormat(string format, object arg0, object arg1, object arg2)
		{
			Log.log.InfoFormat(format, arg0, arg1, arg2);
		}

		public static void InfoFormat(IFormatProvider provider, string format, params object[] args)
		{
			Log.log.InfoFormat( provider, format, args);
		}

		public static void Warn(object message)
		{
			Log.deep++;
			Log.log.Warn(GetSpaces + message);
		}

		public static void Warn(object message, Exception exception)
		{
			Log.deep++;
			Log.log.Warn(GetSpaces + message,  exception);
		}

		public static void WarnFormat(string format, params object[] args)
		{
			Log.log.WarnFormat(format, args);
		}

		public static void WarnFormat(string format, object arg0)
		{
			Log.log.WarnFormat(format, arg0);
		}

		public static void WarnFormat(string format, object arg0, object arg1)
		{
			Log.log.WarnFormat(format, arg0, arg1);
		}

		public static void WarnFormat(string format, object arg0, object arg1, object arg2)
		{
			Log.log.WarnFormat(format, arg0, arg1, arg2);
		}

		public static void WarnFormat(IFormatProvider provider, string format, params object[] args)
		{
			Log.log.WarnFormat( provider, format, args);
		}

		public static void Error(object message)
		{
			Log.deep++;
			Log.log.Error(GetSpaces + message);
		}

		public static void Error(object message, Exception exception)
		{
			Log.deep++;
			Log.log.Error(GetSpaces + message,  exception);
		}

		public static void ErrorFormat(string format, params object[] args)
		{
			Log.log.ErrorFormat(format, args);
		}

		public static void ErrorFormat(string format, object arg0)
		{
			Log.log.ErrorFormat(format, arg0);
		}

		public static void ErrorFormat(string format, object arg0, object arg1)
		{
			Log.log.ErrorFormat(format, arg0, arg1);
		}

		public static void ErrorFormat(string format, object arg0, object arg1, object arg2)
		{
			Log.log.ErrorFormat(format, arg0, arg1, arg2);
		}

		public static void ErrorFormat(IFormatProvider provider, string format, params object[] args)
		{
			Log.log.ErrorFormat( provider, format, args);
		}

		public static void Fatal(object message)
		{
			Log.deep++;
			Log.log.Fatal(GetSpaces + message);
		}

		public static void Fatal(object message, Exception exception)
		{
			Log.deep++;
			Log.log.Fatal(GetSpaces + message,  exception);
		}

		public static void FatalFormat(string format, params object[] args)
		{
			Log.log.FatalFormat(format, args);
		}

		public static void FatalFormat(string format, object arg0)
		{
			Log.log.FatalFormat(format, arg0);
		}

		public static void FatalFormat(string format, object arg0, object arg1)
		{
			Log.log.FatalFormat(format, arg0, arg1);
		}

		public static void FatalFormat(string format, object arg0, object arg1, object arg2)
		{
			Log.log.FatalFormat(format, arg0, arg1, arg2);
		}

		public static void FatalFormat(IFormatProvider provider, string format, params object[] args)
		{ 
			Log.log.FatalFormat( provider, format, args);
		}

		public static bool IsDebugEnabled { get { return Log.log.IsDebugEnabled; } }
		public static bool IsInfoEnabled { get { return Log.log.IsInfoEnabled; } }
		public static bool IsWarnEnabled { get { return Log.log.IsWarnEnabled; } }
		public static bool IsErrorEnabled { get { return Log.log.IsErrorEnabled; } }
		public static bool IsFatalEnabled { get { return Log.log.IsFatalEnabled; } }

		public static void iDone()
		{
			Log.log.iDone();
			Log.deep--;
		}

		public static void iDone(int elapsedSeconds)
		{
			Log.log.iDone(elapsedSeconds);
			Log.deep--;
		}

		public static void iDone(double elapsedMilliseconds)
		{
			Log.log.iDone(elapsedMilliseconds);
			Log.deep--;
		}

		public static void iDone(string additionalInfo)
		{
			Log.log.iDone(additionalInfo);
			Log.deep--;
		}

		public static void InitLogger()
		{
			FileInfo configFile = new FileInfo("logger.config");

			XmlConfigurator.Configure(configFile);
		}

		public static void iDone(this ILog log)
		{
			log.Info(GetSpaces + "done");
		}

		public static void iDone(this ILog log, string additionalInfo)
		{
			log.Info(GetSpaces + $"done, {additionalInfo}");
		}

		public static void iDone(this ILog log, int elapsedSeconds)
		{
			log.Info(GetSpaces + $"done, it takes {elapsedSeconds} sec");
		}

		public static void iDone(this ILog log, double elapsedMilliseconds)
		{
			log.Info(GetSpaces + $"done, it takes {elapsedMilliseconds} ms");
		}
	}
}
