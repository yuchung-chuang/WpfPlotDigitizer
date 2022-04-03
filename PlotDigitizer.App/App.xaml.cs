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
#if DEBUG
			//Test();
#endif
			var window = provider.GetService<Window>();
			window.Show();
		}

		private void ConfigureServices(IServiceCollection services)
		{
			services.AddSingleton<Window, MainWindow>();
			services.AddSingleton<Model>();
			services.AddSingleton<AutoPageTurner>();
			services.AddSingleton<PageManager>();
			services.AddSingleton<LoadPage>();
			services.AddSingleton<AxisLimitPage>();
			services.AddSingleton<AxisPage>();
			services.AddSingleton<FilterPage>();
			services.AddSingleton<EditPage>();
			services.AddSingleton<PreviewPage>();
			services.AddSingleton(provider =>
			{
				return new List<Page>
				{
					provider.GetService<LoadPage>(),
					provider.GetService<AxisLimitPage>(),
					provider.GetService<AxisPage>(),
					provider.GetService<FilterPage>(),
					provider.GetService<EditPage>(),
					provider.GetService<PreviewPage>(),
				};
			});

			services.AddTransient(provider => new ProgressPopup
			{
				Owner = provider.GetService<Window>()
			});
		}

		private void Test()
		{
			var model = provider.GetService<Model>();
			model.InputImage = new BitmapImage(new Uri(@"pack://siteoforigin:,,,/Assets/test_image.png")).ToBitmap().ToImage<Rgba, byte>();
			model.Setting.AxisLimit = new RectangleD(900, 0, 70, 20);
			model.Setting.AxisLogBase = new PointD(0, 0);
			model.Setting.AxisLocation = new Rectangle(138, 100, 632, 399);
			model.Setting.FilterMin = new Rgba(0, 0, 0, byte.MaxValue);
			model.Setting.FilterMax = new Rgba(126, 254, 254, byte.MaxValue);
			model.Setting.DataType = DataType.Discrete;

			provider.GetService<PageManager>().GoToByTypeCommand.Execute(typeof(PreviewPage));
		}
	}
}
