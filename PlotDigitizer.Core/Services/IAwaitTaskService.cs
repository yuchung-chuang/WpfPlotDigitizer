using System;
using System.Threading;
using System.Threading.Tasks;

namespace PlotDigitizer.Core
{
	public interface IAwaitTaskService
	{
		Task<T> RunAsync<T>(Func<CancellationToken, T> func);
		Task RunAsync(Action<CancellationToken> func);
	}
}
