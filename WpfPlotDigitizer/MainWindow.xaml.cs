using MahApps.Metro.Controls;
using static WpfPlotDigitizer.Singletons;

namespace WpfPlotDigitizer
{
  /// <summary>
  /// MainWindow.xaml 的互動邏輯
  /// </summary>
  public partial class MainWindow : MetroWindow
  {
    public MainWindow()
    {
      InitializeComponent();

      DataContext = mainWindowVM;
    }
  }
}
