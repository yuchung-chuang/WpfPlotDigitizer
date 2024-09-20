using Microsoft.Extensions.Logging;

namespace PlotDigitizer.Core
{
	[ProviderAlias("File")]
	public class FileLoggerProvider(string path) : ILoggerProvider
	{
		public ILogger CreateLogger(string categoryName) => new FileLogger(path);

		public void Dispose() { }
	}
}