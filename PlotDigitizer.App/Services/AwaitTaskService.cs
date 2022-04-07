using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Microsoft.Extensions.DependencyInjection;

namespace PlotDigitizer.App
{
	public class AwaitTaskService : IAwaitTaskService
	{
		private readonly IServiceProvider serviceProvider;

		public AwaitTaskService(IServiceProvider serviceProvider)
		{
			this.serviceProvider = serviceProvider;
		}

		public async Task<T> RunAsync<T>(Func<CancellationToken, T> func)
		{
			Mouse.OverrideCursor = Cursors.Wait;

			var cts = new CancellationTokenSource();
			var token = cts.Token;
			var saveTask = new Task<T>(() => func.Invoke(token), token);

			var popup = serviceProvider.GetService<ProgressPopup>();
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
	}
}
