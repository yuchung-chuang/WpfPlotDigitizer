using CycLibrary;
using CycLibrary.Media;
using CycLibrary.MVVM;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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
      //TextBoxBehaviors.TextBox_Error(sender, e); //should not clear, since it will invoke another validation
    }

    private void TextBox_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
    {
      TextBoxBehaviors.TextBox_GotKeyboardFocus(sender, e);
    }

    private void TextBox_GotMouseCapture(object sender, MouseEventArgs e)
    {
      TextBoxBehaviors.TextBox_GotMouseCapture(sender, e);
    }

    private void TextBox_KeyDown(object sender, KeyEventArgs e)
    {
      TextBoxBehaviors.TextBox_KeyDown(sender, e);
    }

    private void TextBox_LostFocus(object sender, RoutedEventArgs e)
    {
      // we manually fire the bindings to invoke validation 
      (sender as TextBox).GetBindingExpression(TextBox.TextProperty).UpdateSource();
      axLimPageVM.IsViewValid = ValidationHelpers.IsValid(this);
    }
  }
}
