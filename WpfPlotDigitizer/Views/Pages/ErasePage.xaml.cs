using CycLibrary.Media;
using System.Windows.Controls;
using static WpfPlotDigitizer.DI;

namespace WpfPlotDigitizer
{
  /// <summary>
  /// ErasePage.xaml 的互動邏輯
  /// </summary>
  public partial class ErasePage : AnimatedPage
  {
    public ErasePage()
    {
      InitializeComponent();

      DataContext = erasePageVM;
    }
  }
}
