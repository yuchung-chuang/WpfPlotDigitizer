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
      appManager.PageManager.TurnTo((int)ApplicationPages.Save);
#endif
    }
  }
}
