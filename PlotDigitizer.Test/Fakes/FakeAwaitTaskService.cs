using System;
using System.Threading;
using System.Threading.Tasks;

namespace PlotDigitizer.Core.Tests.Fakes
{
	/// <summary>
	/// Hand-rolled fake for <see cref="IAwaitTaskService"/>. Runs the work synchronously on the
	/// calling thread so a test never has to poll, and hands out a token the test controls.
	/// </summary>
	internal sealed class FakeAwaitTaskService : IAwaitTaskService
	{
		private readonly CancellationTokenSource cancellation = new();

		public int RunAsyncCallCount { get; private set; }

		/// <summary>Cancels the token handed to the work before it runs.</summary>
		public bool CancelBeforeRun { get; set; }

		public Task<T> RunAsync<T>(Func<CancellationToken, T> func)
		{
			RunAsyncCallCount++;
			return Task.FromResult(func(GetToken()));
		}

		public Task RunAsync(Action<CancellationToken> func)
		{
			RunAsyncCallCount++;
			func(GetToken());
			return Task.CompletedTask;
		}

		public Task<T> RunAsync<T>(Func<CancellationToken, Task<T>> func)
		{
			RunAsyncCallCount++;
			return func(GetToken());
		}

		private CancellationToken GetToken()
		{
			if (CancelBeforeRun) {
				cancellation.Cancel();
			}
			return cancellation.Token;
		}
	}
}
