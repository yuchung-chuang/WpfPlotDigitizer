using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PlotDigitizer.App
{
	public interface IAwaitTaskService
	{
		Task<T> RunAsync<T>(Func<CancellationToken, T> func);
	}
}
