using CycWpfLibrary.Media;
using System.Windows.Controls;
using static WpfPlotDigitizer.DI;

namespace WpfPlotDigitizer
{
  /// <summary>
  /// BrowsePage.xaml 的互動邏輯
  /// </summary>
  public partial class BrowsePage : AnimatedPage
  {
    public BrowsePage()
    {
      InitializeComponent();
      DataContext = browsePageVM;
    }

  }
}
