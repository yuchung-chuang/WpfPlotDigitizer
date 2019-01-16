using System.Windows.Controls;
using static WpfPlotDigitizer.Singletons;

namespace WpfPlotDigitizer
{
  /// <summary>
  /// BrowsePage.xaml 的互動邏輯
  /// </summary>
  public partial class BrowsePage : UserControl
  {
    public BrowsePage()
    {
      InitializeComponent();

      DataContext = browsePageVM;
    }
  }
}
