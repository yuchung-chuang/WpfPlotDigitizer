using CycWpfLibrary;
using CycWpfLibrary.Media;
using Dna;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using static WpfPlotDigitizer.DI;
using CycWpfLibrary.Resources;

namespace WpfPlotDigitizer
{
  /// <summary>
  /// App.xaml 的互動邏輯
  /// </summary>
  public partial class App : Application
  {
    /// <summary>
    /// custom startup so we load the iocContainer immediately after startup
    /// </summary>
    /// <param name="e"></param>
    protected override async void OnStartup(StartupEventArgs e)
    {
      base.OnStartup(e);
      Framework.Construct<DefaultFrameworkConstruction>()
        .AddWpfPlotDigitizerServices()
        .Build();

      Current.MainWindow = mainWindow;
      mainWindow.splashFrame.Content = splashPage;
      Current.MainWindow.Show();

#if DEBUG
      splashPageVM.CompleteCommand.Execute(null);
#endif

      await NativeMethod.WaitAsync(obj => (obj as SplashPageVM).IsComplete, splashPageVM);
      mainWindow.gridMain.Children.Remove(mainWindow.splashFrame);

#if DEBUG
      browsePageVM.PBInput = new Uri(CycResources.PackUri + @"images/data.png").ToPixelBitmap();
      axLimPageVM.AxLim = new Rect(new Point(1e-4, 1e-4), new Point(1e6, 1e7));
      axLimPageVM.AxLogBase = new Point(10, 10);
      appManager.PageManager.TurnTo((int)ApplicationPages.Erase);
#endif
    }
  }
}
