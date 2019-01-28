using CycWpfLibrary.Media;
using Dna;
using System;
using System.Threading;
using System.Windows;
using System.Windows.Media.Imaging;
using static WpfPlotDigitizer.DI;

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
    protected override void OnStartup(StartupEventArgs e)
    {
      base.OnStartup(e);
      Framework.Construct<DefaultFrameworkConstruction>()
        .AddFileLogger()
        .AddWpfPlotDigitizerViewModels()
        .Build();

      Current.MainWindow = mainWindow;
      Current.MainWindow.Show();

#if DEBUG
      browsePageVM.PBInput = new BitmapImage(new Uri($"pack://application:,,,/images/data.png")).ToPixelBitmap();
      axLimPageVM.AxLim = new Rect(new Point(1e-4, 1e-4), new Point(1e6, 1e7));
      axLimPageVM.AxLogBase = new Point(10, 10);
      appManager.PageManager.TurnTo((int)ApplicationPages.Save);
#endif
    }
  }
}
