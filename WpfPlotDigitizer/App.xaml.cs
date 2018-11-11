using CycWpfLibrary.Media;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

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

      IoC.SetUp();
      Current.MainWindow = new MainWindow();

#if DEBUG
      IoC.Get<ImageProcessingVM>().PBInput = new BitmapImage(new Uri($"pack://application:,,,/images/data.png")).ToPixelBitmap();

      var ipvm = IoC.Get<ImageProcessingVM>();

      //applicationVM.PageManager.Index = (int)ApplicationPages.AxisLimit;
#endif 
      
      Current.MainWindow.Show();
    }
  }
}
