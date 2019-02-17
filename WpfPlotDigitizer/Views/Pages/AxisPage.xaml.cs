using CycLibrary.Media;
using System.Windows;
using System.Windows.Controls;
using static WpfPlotDigitizer.DI;
using static CycLibrary.Math;
using CycLibrary;
using System.Windows.Input;
using System.Diagnostics;

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
