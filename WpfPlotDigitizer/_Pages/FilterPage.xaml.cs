using CycWpfLibrary.Media;
using System.Windows.Controls;
using static WpfPlotDigitizer.DI;

namespace WpfPlotDigitizer
{
  /// <summary>
  /// FilterPage.xaml 的互動邏輯
  /// </summary>
  public partial class FilterPage : AnimatedPage
  {
    public FilterPage()
    {
      InitializeComponent();

      DataContext = filterPageVM;
    }
  }
}
