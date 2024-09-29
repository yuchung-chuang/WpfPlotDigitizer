using Microsoft.Extensions.DependencyInjection;
using System.Configuration;
using System.Data;
using System.Windows;

namespace WpfAppTemplate1
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var serviceCollection = new ServiceCollection()
                .AddSingleton<IDemoService, DemoService>()
                .AddSingleton<IWindowService, WindowService>()
                .AddTransient<MainViewModel>();

            var serviceProvider = serviceCollection.BuildServiceProvider();

            var windowService = serviceProvider.GetRequiredService<IWindowService>();
            var mainViewModel = serviceProvider.GetRequiredService<MainViewModel>();
            windowService.ShowMainWindow(mainViewModel);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
        }
    }
}
