using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Microsoft.Extensions.DependencyInjection;
using PlotDigitizer.Core;

namespace PlotDigitizer.App
{
	public class AwaitTaskService : IAwaitTaskService
	{
		private readonly ProgressPopup popup;

		public AwaitTaskService(ProgressPopup popup)
		{
			this.popup = popup;
		}

		public async Task<T> RunAsync<T>(Func<CancellationToken, T> func)
		{
			Mouse.OverrideCursor = Cursors.Wait;

			var cts = new CancellationTokenSource();
			var token = cts.Token;
			var saveTask = new Task<T>(() => func.Invoke(token), token);

			popup.Owner = Application.Current.MainWindow;
			popup.Canceled += (sender, e) => cts.Cancel();
			Debug.WriteLine(popup.GetHashCode());
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

			popup.Owner = Application.Current.MainWindow;
			popup.Canceled += (sender, e) => cts.Cancel();
			popup.Show();

			saveTask.Start();
			await saveTask;
			popup.Close();
			Mouse.OverrideCursor = null;
		}
	}
}
