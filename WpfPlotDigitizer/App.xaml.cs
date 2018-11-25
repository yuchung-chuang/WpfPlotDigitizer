using CycWpfLibrary.Media;
using Dna;
using System;
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

      Current.MainWindow = new MainWindow();

#if DEBUG
      IPManager.PBInput = new BitmapImage(new Uri($"pack://application:,,,/images/data.png")).ToPixelBitmap();
      AppManager.PageManager.TurnNext();
#endif

      Current.MainWindow.Show();
    }
  }
}
