using CycWpfLibrary;
using CycWpfLibrary.Media;
using CycWpfLibrary.Resources;
using CycWpfLibrary.UserControls;
using Dna;
using System;
using System.Globalization;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
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
    protected override async void OnStartup(StartupEventArgs e)
    {
      base.OnStartup(e);
      QuickConverterHelpers.Setup();
      Framework.Construct<DefaultFrameworkConstruction>()
        .AddWpfPlotDigitizerServices()
        .Build();

      appManager.LoadLanguage();
      Current.MainWindow = mainWindow;
      mainWindow.splashFrame.Content = splashPage;
      Current.MainWindow.Show();

#if DEBUG
      splashPageVM.CompleteCommand.Execute(null);
#endif

      await NativeMethod.WaitAsync(obj => (obj as SplashPageVM).IsComplete, splashPageVM);
      mainWindow.gridMain.Children.Remove(mainWindow.splashFrame);

      var pageControl = mainWindow.Resources["pageControl"] as PageControl;
      mainWindow.pageControl = pageControl;
      pageControl.PageAnimated += mainWindow.PageControl_PageAnimated;
      mainWindow.gridMain.Children.Add(pageControl);

#if DEBUG
      browsePageVM.PBInput = new Uri(CycResources.PackUri + @"images/data.png").ToPixelBitmap();
      axLimPageVM.AxLim = new Rect(new Point(1e-4, 1e-4), new Point(1e6, 1e7));
      axLimPageVM.AxLogBase = new Point(10, 0);
      appManager.PageManager.TurnTo((int)ApplicationPages.Filter);
#endif
    }
  }
}
