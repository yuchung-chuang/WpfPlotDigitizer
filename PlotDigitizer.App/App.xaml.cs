using Emgu.CV;
using Emgu.CV.Structure;

using Microsoft.Extensions.DependencyInjection;
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
		private ILogger<App> logger;
		private ServiceProvider serviceProvider;

		public App()
		{
			SetupExceptionHandling();
		}

		protected override void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);
			var splashWindow = new SplashWindow();
			splashWindow.Show();

			var services = new ServiceCollection();
			services.AddTransient<IMessageBoxService, MessageBoxService>()
			.AddTransient<IFileDialogService, FileDialogService>()
			.AddTransient<IAwaitTaskService, AwaitTaskService>()
			.AddTransient<IClipboardService, ClipboardService>()

			.AddSingleton<Model, ModelFacade>()
			.AddSingleton<Setting, SettingFacade>()
			.AddSingleton<InputImageNode>()
			.AddSingleton<CroppedImageNode>()
			.AddSingleton<FilteredImageNode>()
			.AddSingleton<EdittedImageNode>()
			.AddSingleton<PreviewImageNode>()
			.AddSingleton<DataNode>()

			.AddSingleton<AxisLimitNode>()
			.AddSingleton<AxisLogBaseNode>()
			.AddSingleton<AxisLocationNode>()
			.AddSingleton<FilterMinNode>()
			.AddSingleton<FilterMaxNode>()
			.AddSingleton<DataTypeNode>()

			// TODO: ViewModels shouldn't be singleton!!
			.AddSingleton<MainWindowViewModel>()
			.AddSingleton<LoadPageViewModel>()
			.AddSingleton<RangePageViewModel>()
			.AddSingleton<AxisPageViewModel>()
			.AddSingleton<FilterPageViewModel>()
			.AddSingleton<EditPageViewModel>()
			.AddSingleton<DataPageViewModel>()

			.AddLogging(builder =>
			{
				builder.ClearProviders() // to override the default set of logging providers added by the default host
					.AddProvider(new DebugLoggerProvider())
					.AddProvider(new FileLoggerProvider(Path.Combine(Directory.GetCurrentDirectory(), "logs")));
			});

			serviceProvider = services.BuildServiceProvider();
			ConfigureStaticServices();


			logger = serviceProvider.GetService<ILogger<App>>();
			logger?.LogInformation($"{this} started.");

			//initialise mainwindow before testing, so all viewmodels are ready for testing
			MainWindow = new MainWindow(); //need to initialise mainwindow before closing splashWindow, otherwise the application shuts down immidiately as at one moment there is no window at all.
			splashWindow.Close();
			MainWindow.Show();
			logger?.LogInformation($"{MainWindow} Loaded.");

			//Test();
		}


		private void ConfigureStaticServices()
		{
			DI.Resolver = serviceProvider.GetRequiredService;
			Methods.Logger = serviceProvider.GetRequiredService<ILogger<Methods>>();
		}

		private void Test()
		{
			var model = serviceProvider.GetRequiredService<Model>();
			var setting = serviceProvider.GetRequiredService<Setting>();
			model.InputImage = new BitmapImage(new Uri(@"pack://application:,,,/Assets/test_image.png")).ToBitmap().ToImage<Rgba, byte>();

			var settingTmp = new Setting
			{
				AxisLimit = new RectangleD(900, 0, 70, 20),
				AxisLocation = new RectangleD(138, 100, 632, 399),
				FilterMin = new Rgba(0, 0, 0, byte.MaxValue),
				FilterMax = new Rgba(126, 254, 254, byte.MaxValue),
				DataType = DataType.Discrete
			};
			setting.Load(settingTmp);

			(MainWindow as MainWindow).navigation.Navigate(typeof(DataPage));
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