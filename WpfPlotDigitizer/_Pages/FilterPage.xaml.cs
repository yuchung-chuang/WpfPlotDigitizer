using System.Windows.Controls;
using static WpfPlotDigitizer.DI;

namespace WpfPlotDigitizer
{
  /// <summary>
  /// FilterPage.xaml 的互動邏輯
  /// </summary>
  public partial class FilterPage : Page
  {
    public FilterPage()
    {
      InitializeComponent();

      DataContext = filterPageVM;
    }
  }
}
