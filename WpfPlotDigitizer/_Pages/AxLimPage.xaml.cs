using System.Windows.Controls;
using static WpfPlotDigitizer.DI;

namespace WpfPlotDigitizer
{
  /// <summary>
  /// AxisLimitPage.xaml 的互動邏輯
  /// </summary>
  public partial class AxLimPage : UserControl
  {
    public AxLimPage()
    {
      InitializeComponent();

      DataContext = axLimPageVM;
    }
  }
}
