using CycWpfLibrary.CustomControls;
using CycWpfLibrary.Resource;
using static WpfPlotDigitizer.DI;

namespace WpfPlotDigitizer
{
  /// <summary>
  /// AxLimPopup.xaml 的互動邏輯
  /// </summary>
  public partial class LangPopup : PopupWindow
  {
    public LangPopup()
    {
      InitializeComponent();
      gridMain.DataContext = this;

      switch (appManager.Language)
      {
        default:
        case CycResources.en_US:
          enButton.IsChecked = true;
          break;
        case CycResources.zh_TW:
          zhButton.IsChecked = true;
          break;
      }
    }

    private void enButton_Checked(object sender, System.Windows.RoutedEventArgs e)
    {
      appManager.Language = CycResources.en_US;
    }

    private void zhButton_Checked(object sender, System.Windows.RoutedEventArgs e)
    {
      appManager.Language = CycResources.zh_TW;
    }
  }
}
