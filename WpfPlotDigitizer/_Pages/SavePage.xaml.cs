using System.Windows.Controls;

namespace WpfPlotDigitizer
{
  /// <summary>
  /// SavePage.xaml 的互動邏輯
  /// </summary>
  public partial class SavePage : Page
  {
    public SavePage()
    {
      InitializeComponent();
      DataContext = DI.savePageVM;
    }
  }
}
