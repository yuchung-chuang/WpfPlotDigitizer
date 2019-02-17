using CycLibrary.CustomControls;
using System.Diagnostics;
using System.Windows;

namespace WpfPlotDigitizer
{
  /// <summary>
  /// AxLimPopup.xaml 的互動邏輯
  /// </summary>
  public partial class AboutPopup : PopupWindow
  {
    public AboutPopup()
    {
      InitializeComponent();
    }

    private void githubButton_Click(object sender, RoutedEventArgs e)
    {
      Process.Start("https://github.com/alex1392");
    }

    private void websiteButton_Click(object sender, RoutedEventArgs e)
    {
      Process.Start("https://yuchung-chuang.webnode.tw/");
    }

    private void blogButton_Click(object sender, RoutedEventArgs e)
    {
      Process.Start("https://yuchungchuang.wordpress.com/");
    }

    private void emailButton_Click(object sender, RoutedEventArgs e)
    {
      Process.Start("mailto:yuchung.chuang@gmail.com?subject=[WpfPlotDigitizer]Report");
    }
  }
}
