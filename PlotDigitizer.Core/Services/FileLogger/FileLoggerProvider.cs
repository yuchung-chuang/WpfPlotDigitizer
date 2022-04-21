using Microsoft.Extensions.Logging;

namespace PlotDigitizer.Core
{
	[ProviderAlias("File")]
	public class FileLoggerProvider : ILoggerProvider
	{
		private readonly string path;

		public FileLoggerProvider(string _path) => path = _path;

		public ILogger CreateLogger(string categoryName) => new FileLogger(path);

		public void Dispose()
		{
		}
	}
}