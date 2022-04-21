using PlotDigitizer.Core;

using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace PlotDigitizer.App
{
	public class AwaitTaskService : IAwaitTaskService
	{
		private CancellationToken token;
		private ProgressPopup popup;

		private void Prepare()
		{
			Mouse.OverrideCursor = Cursors.Wait;

			var cts = new CancellationTokenSource();
			token = cts.Token;

			popup = new ProgressPopup
			{
				Owner = Application.Current.MainWindow
			};
			popup.Canceled += (sender, e) => cts.Cancel();
			popup.Show();
		}

		private void Cleanup()
		{
			popup.Close();
			Mouse.OverrideCursor = null;
		}

		public async Task<T> RunAsync<T>(Func<CancellationToken, T> func)
		{
			Prepare();
			var saveTask = new Task<T>(() => func.Invoke(token), token);
			saveTask.Start();
			var result = await saveTask;
			Cleanup();

			return result;
		}

		public async Task RunAsync(Action<CancellationToken> func)
		{
			Prepare();
			var saveTask = new Task(() => func.Invoke(token), token);
			saveTask.Start();
			await saveTask;
			Cleanup();
		}

		public async Task<T> RunAsync<T>(Func<CancellationToken, Task<T>> func)
		{
			Prepare();
			var result = await func.Invoke(token);
			Cleanup();
			return result;
		}
	}
}