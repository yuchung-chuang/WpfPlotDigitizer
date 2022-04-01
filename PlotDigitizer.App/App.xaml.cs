using Emgu.CV;
using Emgu.CV.Structure;
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

            provider.GetService<AutoPageTurner>(); // simply trigger the creation of auto-page turner, it will do it's job
            //test();
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

        private void test()
        {
            var model = provider.GetService<Model>();
            model.InputImage = new BitmapImage(new Uri(@"C:\Users\yxc826\source\repos\alex1392\PlotDigitizer\images\Screenshot 2021-06-26 230901.png")).ToBitmap().ToImage<Rgba, byte>();
            var pagaManager = provider.GetService<PageManager>();
            pagaManager.GoToByTypeCommand.Execute(typeof(EditPage));
        }
    }
}
