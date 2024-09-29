using Emgu.CV;
using Emgu.CV.Structure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Configuration;
using PlotDigitizer.Core;

using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace PlotDigitizer.WPF
{
    public partial class App : Application
	{
		private ILogger<App> logger;
		private ServiceProvider serviceProvider;

		protected override void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);
			
            SetupExceptionHandling();

            // Build Configuration
            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())   // Set the base path to the current directory
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true) // Load appsettings.json
                .Build();

			var services = new ServiceCollection()
				.AddSingleton(configuration)
				.AddKeyedSingleton<IOcrService, OcrService>("Numerical")
				.Configure<OcrSettings>("Numerical", settings =>
				{
                    var section = configuration.GetSection("OCR").GetSection("Numerical");
					settings.DataPath = section[nameof(settings.DataPath)];
					settings.Language = section[nameof(settings.Language)];
					settings.WhiteList = section[nameof(settings.WhiteList)];
                })
                .AddKeyedSingleton<IOcrService, OcrService>("Text")
                .Configure<OcrSettings>("Text", settings =>
                {
                    var section = configuration.GetSection("OCR").GetSection("Text");
                    settings.DataPath = section[nameof(settings.DataPath)];
                    settings.Language = section[nameof(settings.Language)];
                    settings.WhiteList = section[nameof(settings.WhiteList)];
                })
                .AddTransient<IMessageBoxService, MessageBoxService>()
				.AddTransient<IFileDialogService, FileDialogService>()
				.AddTransient<IAwaitTaskService, AwaitTaskService>()
				.AddTransient<IClipboardService, ClipboardService>()
				.AddSingleton<IPageService, PageService>()
				.AddTransient<IImageService, EmguCvService>()
				.AddTransient<IWindowService, WindowService>()
				.AddViewModels()
				.AddModel()
			
				.AddLogging(builder =>
				{
					builder.ClearProviders() // to override the default set of logging providers added by the default host
						.AddProvider(new DebugLoggerProvider())
						.AddProvider(new FileLoggerProvider(Path.Combine(Directory.GetCurrentDirectory(), "logs")))
						.AddConfiguration(configuration.GetSection("Logging"));
				});

			serviceProvider = services.BuildServiceProvider();

			var splashWindow = new SplashWindow();
			splashWindow.Show();
						
			logger = serviceProvider.GetService<ILogger<App>>();
			logger?.LogInformation($"{this} started.");

			// initialise mainwindow before testing, so all viewmodels are ready for testing
			// need to initialise mainwindow before closing splashWindow, otherwise the application shuts down immidiately as at one moment there is no window at all.
			var mainViewModel = serviceProvider.GetRequiredService<MainViewModel>();

			MainWindow = new MainWindow
			{
				DataContext = mainViewModel,
			};
			MainWindow.Show();
			splashWindow.Close();
			logger?.LogInformation("MainWindow Loaded.");

			var pageService = serviceProvider.GetRequiredService<IPageService>();
			pageService.Initialise();
			logger?.LogInformation("Page Loaded.");

			if (configuration["RunTest"].ToLower() == true.ToString()) {
                Test();
            }
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

			//(MainWindow as MainWindow).navigation.Navigate(typeof(DataPage));
		}

		/// <summary>
		/// Exception handling should be set before doing anything else, so all exception can be handled internally without crashing the application.
		/// </summary>
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