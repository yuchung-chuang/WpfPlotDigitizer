using CycWpfLibrary;
using System.Windows.Controls;
using System.Windows.Input;
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
  }
}
