using System.Linq;
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

    private void TextBox_Error(object sender, ValidationErrorEventArgs e)
    {
      var tb = sender as TextBox;
      if (Validation.GetHasError(tb))
      {
        axLimPageVM[tb.Tag as string] = null;
      }
    }
  }
}
