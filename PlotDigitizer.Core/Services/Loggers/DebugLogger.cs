using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;

namespace PlotDigitizer.Core
{
    public class DebugLogger : ILogger
	{
		public IDisposable BeginScope<TState>(TState state) where TState : notnull => default!;
		public bool IsEnabled(LogLevel logLevel) => true;
		public void Log<TState>(LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception exception,
            Func<TState, Exception, string> formatter)
		{
			if (!IsEnabled(logLevel)) {
				return;
			}
			if (formatter == null) {
				return;
			}

			var n = Environment.NewLine;
			var excepctionMessage = "";
			if (exception != null)
				excepctionMessage = n + exception.GetType() + ": " + exception.Message + n + exception.StackTrace + n;

            var message = logLevel.ToString() + ": " + DateTime.Now.ToString() + " " + formatter(state, exception) + n + excepctionMessage;
            Debug.WriteLine(message);
		}

		
	}
}