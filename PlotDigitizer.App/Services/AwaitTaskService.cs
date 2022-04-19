using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using PlotDigitizer.Core;

namespace PlotDigitizer.App
{
	public class AwaitTaskService : IAwaitTaskService
	{
		public async Task<T> RunAsync<T>(Func<CancellationToken, T> func)
		{
			Mouse.OverrideCursor = Cursors.Wait;

			var cts = new CancellationTokenSource();
			var token = cts.Token;
			var saveTask = new Task<T>(() => func.Invoke(token), token);

			var popup = new ProgressPopup
			{
				Owner = Application.Current.MainWindow
			};
			popup.Canceled += (sender, e) => cts.Cancel();
			popup.Show();

			saveTask.Start();
			var result = await saveTask;
			popup.Close();
			Mouse.OverrideCursor = null;
			
			return result;
		}

		public async Task RunAsync(Action<CancellationToken> func)
		{
			Mouse.OverrideCursor = Cursors.Wait;

			var cts = new CancellationTokenSource();
			var token = cts.Token;
			var saveTask = new Task(() => func.Invoke(token), token);

			var popup = new ProgressPopup
			{
				Owner = Application.Current.MainWindow
			};
			popup.Canceled += (sender, e) => cts.Cancel();
			popup.Show();

			saveTask.Start();
			await saveTask;
			popup.Close();
			Mouse.OverrideCursor = null;
		}
	}
}
