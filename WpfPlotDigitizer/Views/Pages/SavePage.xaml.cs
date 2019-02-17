using CycLibrary.Media;
using System.Windows.Controls;

namespace WpfPlotDigitizer
{
  /// <summary>
  /// SavePage.xaml 的互動邏輯
  /// </summary>
  public partial class SavePage : AnimatedPage
  {
    public SavePage()
    {
      InitializeComponent();
      DataContext = DI.savePageVM;
    }
  }
}
