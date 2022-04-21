using Microsoft.Extensions.Logging;

namespace PlotDigitizer.Core
{
	public static class FileLoggerExtensions
	{
		public static ILoggerFactory AddFile(this ILoggerFactory factory, string filePath)
		{
			factory.AddProvider(new FileLoggerProvider(filePath));
			return factory;
		}

		public static ILoggingBuilder AddFile(this ILoggingBuilder builder, string filePath)
		{
			builder.AddProvider(new FileLoggerProvider(filePath));
			return builder;
		}
	}
}