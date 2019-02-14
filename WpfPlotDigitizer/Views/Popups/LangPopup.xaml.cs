using CycWpfLibrary.CustomControls;
using CycWpfLibrary.Resources;
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
        case "en-US":
          enButton.IsChecked = true;
          break;
        case "zh-TW":
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
