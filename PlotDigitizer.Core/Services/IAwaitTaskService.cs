using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PlotDigitizer.Core
{
	public interface IAwaitTaskService
	{
		Task<T> RunAsync<T>(Func<CancellationToken, T> func);
		Task RunAsync(Action<CancellationToken> func);
		Task<T> RunAsync<T>(Func<CancellationToken, Task<T>> func);
	}
}
