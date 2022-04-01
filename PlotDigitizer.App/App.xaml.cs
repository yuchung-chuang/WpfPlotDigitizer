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
		protected override void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);

			// DI registeration
			var services = new ServiceCollection();
			ConfigureServices(services);
			var provider = services.BuildServiceProvider();

			provider.GetService<AutoPageTurner>(); // simply trigger the creation of auto-page turner, it will do it's job
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
			services.AddSingleton(provider =>
			{
				return new List<Page>
				{
					provider.GetService<LoadPage>(),
					provider.GetService<AxisLimitPage>(),
					provider.GetService<AxisPage>(),
					provider.GetService<FilterPage>(),
					provider.GetService<EditPage>(),
				};
			});
		}
	}
}
