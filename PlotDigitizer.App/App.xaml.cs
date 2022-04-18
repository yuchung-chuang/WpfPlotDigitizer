using Emgu.CV;
using Emgu.CV.Structure;
using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PlotDigitizer.Core;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace PlotDigitizer.App
{
	public partial class App : Application
	{
		private IHost host;

		public App()
		{
			Startup += App_Startup;
			Exit += App_Exit;
		}

		private async void App_Startup(object sender, StartupEventArgs e)
		{
			// app host
			host = Host.CreateDefaultBuilder(e.Args)
				.ConfigureAppConfiguration((context, builder) =>
				{
					builder.SetBasePath(context.HostingEnvironment.ContentRootPath)
						.AddJsonFile("appsettings.json", optional: false);
				})
				.ConfigureServices((context, services) =>
				{
					services.AddTransient<IMessageBoxService, MessageBoxService>()
					.AddTransient<IFileDialogService, FileDialogService>()
					.AddTransient<IAwaitTaskService, AwaitTaskService>()
					.AddTransient<IClipboard, Clipboard>()
					.AddSingleton<IModel, Model2>()
					.AddSingleton<ISetting, Setting2>()
					.AddSingleton<Model>()

					.AddTransient<ProgressPopup>()
					.AddTransient<AutoPageTurner>()

					.AddViewModels()
					.AddModelNodes();
					
				})
				.ConfigureLogging((context, builder) =>
				{
					builder.ClearProviders() // to override the default set of logging providers added by the default host
						.AddDebug()
						.AddFile(Path.Combine(Directory.GetCurrentDirectory(), "logs"));
				})
				.Build();
			await host.StartAsync();

			var logger = host.Services.GetService<ILogger<App>>();
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

			// need to trigger the creation of these viewmodels before showing the views to set up model updating pipeline beforehand
			var vm = services.GetRequiredService<MainWindowViewModel>();
			vm.PageManager.Initialise(new List<PageViewModelBase>
			{
				services.GetRequiredService<LoadPageViewModel       >(),
				services.GetRequiredService<AxisLimitPageViewModel  >(),
				services.GetRequiredService<AxisPageViewModel       >(),
				services.GetRequiredService<FilterPageViewModel     >(),
				//services.GetRequiredService<EditPageViewModel		>(),
				services.GetRequiredService<PreviewPageViewModel    >(),
			});
		}
		private void Test()
		{
			var provider = host.Services;
			var model = provider.GetRequiredService<Model>();
			model.InputImage = new BitmapImage(new Uri(@"pack://siteoforigin:,,,/Assets/test_image.png")).ToBitmap().ToImage<Rgba, byte>();
			model.Setting.AxisLimit = new RectangleD(900, 0, 70, 20);
			model.Setting.AxisLocation = new Rectangle(138, 100, 632, 399);
			model.Setting.FilterMin = new Rgba(0, 0, 0, byte.MaxValue);
			model.Setting.FilterMax = new Rgba(126, 254, 254, byte.MaxValue);
			model.Setting.DataType = DataType.Discrete;

			provider.GetRequiredService<MainWindowViewModel>().PageManager.GoToByTypeCommand.Execute(typeof(PreviewPageViewModel));
		}
	}
}
