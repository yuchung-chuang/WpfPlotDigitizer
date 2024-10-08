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

            // Log startup event
            logger?.LogDebug("Application OnStartup called.");

            SetupExceptionHandling();

            // Build Configuration
            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())   // Set the base path to the current directory
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true) // Load appsettings.json
                .Build();

            logger?.LogInformation("Configuration loaded.");

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
                .AddTransient<IImageService, EmguCvService>()
                .AddTransient<IMessageBoxService, MessageBoxService>()
                .AddTransient<IFileDialogService, FileDialogService>()
                .AddTransient<IAwaitTaskService, AwaitTaskService>()
                .AddTransient<IClipboardService, ClipboardService>()
                .AddSingleton<IPageService, PageService>()
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

            logger = serviceProvider.GetService<ILogger<App>>();
            logger?.LogInformation($"{this} started.");

            var splashWindow = new SplashWindow();
            splashWindow.Show();
            logger?.LogDebug("Splash screen shown.");

            // Initialise MainWindow before testing to ensure all ViewModels are ready.
            var mainViewModel = serviceProvider.GetRequiredService<MainViewModel>();

            MainWindow = new MainWindow
            {
                DataContext = mainViewModel,
            };
            MainWindow.Show();
            logger?.LogInformation("MainWindow displayed.");

            splashWindow.Close();
            logger?.LogDebug("Splash screen closed.");

            var pageService = serviceProvider.GetRequiredService<IPageService>();
            pageService.Initialise();
            logger?.LogInformation("Page service initialized.");

            if (bool.TryParse(configuration["RunTest"], out var runTest) && runTest) {
                logger?.LogDebug("Test mode enabled. Running test.");
                Test();
            }
        }

        private void Test()
        {
            try {
                logger?.LogDebug("Test started.");

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

                logger?.LogInformation("Test completed successfully.");
            }
            catch (Exception ex) {
                logger?.LogError(ex, "Error during test execution.");
            }
        }

        private void SetupExceptionHandling()
        {
            // Log that exception handling is being set up
            logger?.LogDebug("Setting up global exception handling.");

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

            logger?.LogInformation("Global exception handling setup complete.");
        }

        private void LogUnhandledException(Exception exception, string source)
        {
            var message = $"Unhandled exception ({source})";
            try {
                var assemblyName = Assembly.GetExecutingAssembly().GetName();
                message = string.Format("Unhandled exception in {0} v{1}", assemblyName.Name, assemblyName.Version);
            }
            catch (Exception ex) {
                logger?.LogError(ex, "Exception in LogUnhandledException");
            }
            finally {
                logger?.LogError(exception, message);
            }
        }
    }
}
