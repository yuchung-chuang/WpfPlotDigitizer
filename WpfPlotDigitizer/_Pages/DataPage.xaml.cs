using System.Windows.Controls;

namespace WpfPlotDigitizer
{
  /// <summary>
  /// SizePage.xaml 的互動邏輯
  /// </summary>
  public partial class DataPage : Page
  {
    public DataPage()
    {
      InitializeComponent();
      DataContext = DI.dataPageVM;
    }
  }
}
