using System.Windows.Controls;
using static WpfPlotDigitizer.DI;

namespace WpfPlotDigitizer
{
  /// <summary>
  /// AxisPage.xaml 的互動邏輯
  /// </summary>
  public partial class AxisPage : UserControl
  {
    public AxisPage()
    {
      InitializeComponent();

      DataContext = axisPageVM;
    }
  }
}
