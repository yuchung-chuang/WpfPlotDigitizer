using System.Windows.Controls;
using static WpfPlotDigitizer.Singletons;

namespace WpfPlotDigitizer
{
  /// <summary>
  /// AxisLimitPage.xaml 的互動邏輯
  /// </summary>
  public partial class AxisLimitPage : UserControl
  {
    public AxisLimitPage()
    {
      InitializeComponent();

      DataContext = axisLimitPageVM;
    }
  }
}
