using Microsoft.Extensions.Logging;
using System;
using System.IO;

namespace PlotDigitizer.Core
{
	public class FileLogger : ILogger
	{
		private readonly string path;
		private static readonly object key = new object();
		public FileLogger(string path)
		{
			this.path = path;
		}
		public IDisposable BeginScope<TState>(TState state) => default!;

		public bool IsEnabled(LogLevel logLevel) => true;

		public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
		{
			if (!IsEnabled(logLevel)) {
				return;
			}
			if (formatter != null) {
				lock (key) {
					if (!Directory.Exists(path)) { 
						Directory.CreateDirectory(path); 
					}
					var fullFilePath = Path.Combine(path, DateTime.Now.ToString("yyyy-MM-dd") + "_log.txt");
					var n = Environment.NewLine;
					var exc = "";
					if (exception != null) 
						exc = n + exception.GetType() + ": " + exception.Message + n + exception.StackTrace + n;
					File.AppendAllText(fullFilePath, logLevel.ToString() + ": " + DateTime.Now.ToString() + " " + formatter(state, exception) + n + exc);
				}
			}
		}
	}
}
