using Microsoft.Extensions.Logging;

using System;
using System.IO;

namespace PlotDigitizer.Core
{
    public class FileLogger(string path) : ILogger
    {
        private static readonly object key = new();

        public IDisposable BeginScope<TState>(TState state) => default!;

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
            lock (key) {
                if (!Directory.Exists(path)) {
                    Directory.CreateDirectory(path);
                }
                var fullFilePath = Path.Combine(path, DateTime.Now.ToString("yyyy_MM_dd") + ".log");
                var n = Environment.NewLine;
                var exc = "";
                if (exception != null)
                    exc = n + exception.GetType() + ": " + exception.Message + n + exception.StackTrace + n;
                File.AppendAllText(fullFilePath, logLevel.ToString() + ": " + DateTime.Now.ToString() + " " + formatter(state, exception) + n + exc);
            }
        }
    }
}