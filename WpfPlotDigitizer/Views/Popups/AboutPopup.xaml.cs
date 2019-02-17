using CycWpfLibrary.CustomControls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

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
