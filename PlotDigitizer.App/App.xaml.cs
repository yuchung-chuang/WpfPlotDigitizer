using Emgu.CV;
using Emgu.CV.Structure;
using Microsoft.Extensions.DependencyInjection;
using PlotDigitizer.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace PlotDigitizer.App
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		private ServiceProvider provider;

		protected override void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);

			// DI registeration
			var services = new ServiceCollection();
			ConfigureServices(services);
			provider = services.BuildServiceProvider();

			// simply trigger the creation of auto-page turner, it will do it's job
			provider.GetService<AutoPageTurner>();

			// need to trigger the creation of these viewmodels before showing the views to set up model updating pipeline beforehand
			provider.GetService<MainWindowViewModel>();
			provider.GetService<LoadPageViewModel>();
			provider.GetService<AxisLimitPageViewModel>();
			provider.GetService<AxisPageViewModel>();
			provider.GetService<FilterPageViewModel>();
			provider.GetService<EditPageViewModel>();
			provider.GetService<PreviewPageViewModel>();

			var window = provider.GetService<MainWindow>();
			MainWindow = window;
#if DEBUG
			Test();
#endif
			window.Show();
		}

		private void ConfigureServices(IServiceCollection services)
		{
			services.AddTransient<IMessageBoxService, MessageBoxService>();
			services.AddTransient<IFileDialogService, FileDialogService>();
			services.AddTransient<IAwaitTaskService, AwaitTaskService>();
			services.AddTransient<ProgressPopup>();
			services.AddTransient<AutoPageTurner>();

			services.AddTransient<MainWindow>();
			services.AddTransient<LoadPage>();
			services.AddTransient<AxisLimitPage>();
			services.AddTransient<AxisPage>();
			services.AddTransient<FilterPage>();
			services.AddTransient<EditPage>();
			services.AddTransient<PreviewPage>();

			services.AddSingleton<PageManager>();
			services.AddSingleton<Model>();
			services.AddSingleton<MainWindowViewModel>();
			services.AddSingleton<LoadPageViewModel>();
			services.AddSingleton<AxisLimitPageViewModel>();
			services.AddSingleton<AxisPageViewModel>();
			services.AddSingleton<FilterPageViewModel>();
			services.AddSingleton<EditPageViewModel>();
			services.AddSingleton<PreviewPageViewModel>();
		}

		private void Test()
		{
			var model = provider.GetService<Model>();
			model.InputImage = new BitmapImage(new Uri(@"pack://siteoforigin:,,,/Assets/test_image.png")).ToBitmap().ToImage<Rgba, byte>();
			model.Setting.AxisLimit = new RectangleD(900, 0, 70, 20);
			model.Setting.AxisLocation = new Rectangle(138, 100, 632, 399);
			model.Setting.FilterMin = new Rgba(0, 0, 0, byte.MaxValue);
			model.Setting.FilterMax = new Rgba(126, 254, 254, byte.MaxValue);
			model.Setting.DataType = DataType.Discrete;

			provider.GetService<PageManager>().GoToByTypeCommand.Execute(typeof(EditPage));
		}
	}
}
