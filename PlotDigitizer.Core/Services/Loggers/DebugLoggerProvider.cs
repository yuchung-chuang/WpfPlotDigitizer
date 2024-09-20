using Microsoft.Extensions.Logging;

namespace PlotDigitizer.Core
{
	public class DebugLoggerProvider : ILoggerProvider
	{
		public ILogger CreateLogger(string categoryName) => new DebugLogger();

		public void Dispose() { }
	}
}