using Emgu.CV;
using Emgu.CV.Structure;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using PlotDigitizer.Core;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace PlotDigitizer.App
{
	public partial class App : Application
	{
		private IHost host;
		private ILogger<App> logger;

		public App()
		{
			Startup += App_Startup;
			Exit += App_Exit;
			SetupExceptionHandling();
		}

		private async void App_Startup(object sender, StartupEventArgs e)
		{
			var splashWindow = new SplashWindow();
			splashWindow.Show();

			// app host
			host = Host.CreateDefaultBuilder()
				.ConfigureServices((context, services) =>
				{
					services.AddTransient<IMessageBoxService, MessageBoxService>()
					.AddTransient<IFileDialogService, FileDialogService>()
					.AddTransient<IAwaitTaskService, AwaitTaskService>()
					.AddTransient<IClipboardService, ClipboardService>()

					.AddSingleton<AutoPageTurner>()
					.AddModel()
					.AddViewModels();
				})
				.ConfigureLogging((context, builder) =>
				{
					builder.ClearProviders() // to override the default set of logging providers added by the default host
						.AddDebug()
						.AddFile(Path.Combine(Directory.GetCurrentDirectory(), "logs"));
				})
				.Build();
			await host.StartAsync();

			logger = host.Services.GetService<ILogger<App>>();
			var config = host.Services.GetRequiredService<IConfiguration>();

			logger?.LogInformation($"{this} started.");

			InitialiseServices();

			var isRunTest = config.GetSection("AppSettings").GetValue<bool>("RunTest");
			if (isRunTest)
				Test();

			MainWindow = new MainWindow
			{
				DataContext = host.Services.GetRequiredService<MainWindowViewModel>()
			};
			splashWindow.Close();
			MainWindow.Show();
			logger?.LogInformation($"{MainWindow} Loaded.");
		}

		private async void App_Exit(object sender, ExitEventArgs e)
		{
			await host.StopAsync(TimeSpan.FromSeconds(5));
			host.Dispose();
		}

		private void InitialiseServices()
		{
			var services = host.Services;
			// simply trigger the creation of auto-page turner, it will do it's job
			services.GetRequiredService<AutoPageTurner>();

			var vm = services.GetRequiredService<MainWindowViewModel>();
			vm.PageManager.Initialise(new List<PageViewModelBase>
			{
				services.GetRequiredService<LoadPageViewModel       >(),
				services.GetRequiredService<AxisLimitPageViewModel  >(),
				services.GetRequiredService<AxisPageViewModel       >(),
				services.GetRequiredService<FilterPageViewModel     >(),
				services.GetRequiredService<EditPageViewModel       >(),
				services.GetRequiredService<PreviewPageViewModel    >(),
			});
		}

		private void Test()
		{
			var provider = host.Services;
			var model = provider.GetRequiredService<Model>();
			var setting = provider.GetRequiredService<Setting>();
			model.InputImage = new BitmapImage(new Uri(@"pack://application:,,,/Assets/test_image.png")).ToBitmap().ToImage<Rgba, byte>();

			var settingTmp = new Setting
			{
				AxisLimit = new RectangleD(900, 0, 70, 20),
				AxisLocation = new Rectangle(138, 100, 632, 399),
				FilterMin = new Rgba(0, 0, 0, byte.MaxValue),
				FilterMax = new Rgba(126, 254, 254, byte.MaxValue),
				DataType = DataType.Discrete
			};
			setting.Load(settingTmp);

			var mainWindowViewModel = provider.GetRequiredService<MainWindowViewModel>();
			mainWindowViewModel.PageManager.GoToByTypeCommand.Execute(typeof(EditPageViewModel));
		}

		private void SetupExceptionHandling()
		{
			AppDomain.CurrentDomain.UnhandledException += (s, e) =>
				LogUnhandledException((Exception)e.ExceptionObject, "AppDomain.CurrentDomain.UnhandledException");

			DispatcherUnhandledException += (s, e) =>
			{
				LogUnhandledException(e.Exception, "Application.Current.DispatcherUnhandledException");
				e.Handled = true;
			};

			TaskScheduler.UnobservedTaskException += (s, e) =>
			{
				LogUnhandledException(e.Exception, "TaskScheduler.UnobservedTaskException");
				e.SetObserved();
			};
		}

		private void LogUnhandledException(Exception exception, string source)
		{
			var message = $"Unhandled exception ({source})";
			try {
				var assemblyName = Assembly.GetExecutingAssembly().GetName();
				message = string.Format("Unhandled exception in {0} v{1}", assemblyName.Name, assemblyName.Version);
			}
			catch (Exception ex) {
				logger.LogError(ex, "Exception in LogUnhandledException");
			}
			finally {
				logger.LogError(exception, message);
			}
		}
	}
}