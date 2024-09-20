using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.IO;

namespace PlotDigitizer.Core
{
	public class DebugLogger : ILogger
	{
		public IDisposable BeginScope<TState>(TState state) where TState : notnull => default!;
		public bool IsEnabled(LogLevel logLevel) => true;
		public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
		{
			if (!IsEnabled(logLevel)) {
				return;
			}
			if (formatter == null) {
				return;
			}

			var n = Environment.NewLine;
			var exc = "";
			if (exception != null)
				exc = n + exception.GetType() + ": " + exception.Message + n + exception.StackTrace + n;
			Debug.WriteLine(logLevel.ToString() + ": " + DateTime.Now.ToString() + " " + formatter(state, exception) + n + exc);
		}
	}
}