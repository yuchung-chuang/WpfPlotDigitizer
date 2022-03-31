using Microsoft.Extensions.DependencyInjection;
using PlotDigitizer.Core;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace PlotDigitizer.App
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		private AutoPageTurner pageTurner;

		protected override void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);

			// DI registeration
			var services = new ServiceCollection();
			ConfigureServices(services);
			var provider = services.BuildServiceProvider();

			pageTurner = provider.GetService<AutoPageTurner>();
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
			services.AddSingleton<List<Page>>(provider =>
			{
				return new List<Page>
				{
					provider.GetService<LoadPage>(),
					provider.GetService<AxisLimitPage>(),
				};
			});
		}
	}
}
