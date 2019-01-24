using CycWpfLibrary.Media;
using System.Windows.Controls;
using static WpfPlotDigitizer.DI;

namespace WpfPlotDigitizer
{
  /// <summary>
  /// AxisPage.xaml 的互動邏輯
  /// </summary>
  public partial class AxisPage : AnimatedPage
  {
    public AxisPage()
    {
      InitializeComponent();

      DataContext = axisPageVM;
    }
  }
}
