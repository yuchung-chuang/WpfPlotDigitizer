using CycLibrary.Media;
using System.Windows.Controls;

namespace WpfPlotDigitizer
{
  /// <summary>
  /// SizePage.xaml 的互動邏輯
  /// </summary>
  public partial class DataPage : AnimatedPage
  {
    public DataPage()
    {
      InitializeComponent();
      DataContext = DI.dataPageVM;
    }
  }
}
