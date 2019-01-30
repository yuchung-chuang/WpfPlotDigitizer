using CycWpfLibrary.Media;
using System.Linq;
using System.Windows.Controls;
using static WpfPlotDigitizer.DI;

namespace WpfPlotDigitizer
{
  /// <summary>
  /// AxisLimitPage.xaml 的互動邏輯
  /// </summary>
  public partial class AxLimPage : AnimatedPage
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

    private void TextBox_GotKeyboardFocus(object sender, System.Windows.Input.KeyboardFocusChangedEventArgs e)
    {
      (sender as TextBox).SelectAll();
    }

    private void TextBox_GotMouseCapture(object sender, System.Windows.Input.MouseEventArgs e)
    {
      (sender as TextBox).SelectAll();
    }
  }
}
