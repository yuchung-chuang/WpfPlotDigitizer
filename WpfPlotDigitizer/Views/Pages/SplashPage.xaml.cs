using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using static WpfPlotDigitizer.DI;

namespace WpfPlotDigitizer
{
  /// <summary>
  /// SplashPage.xaml 的互動邏輯
  /// </summary>
  public partial class SplashPage : Page
  {
    public SplashPage()
    {
      InitializeComponent();
      DataContext = splashPageVM;
    }

    /// <summary>
    /// Cannot bind command to event of storyboard (since it is freezed)
    /// </summary>
    private void Storyboard_Completed(object sender, EventArgs e)
    {
      splashPageVM.CompleteCommand.Execute(null);
    }

    private void page_MouseDown(object sender, MouseButtonEventArgs e)
    {
      splashPageVM.CompleteCommand.Execute(null);
    }
    private void page_KeyDown(object sender, KeyEventArgs e)
    {
      splashPageVM.CompleteCommand.Execute(null);
    }

    private void page_Loaded(object sender, RoutedEventArgs e)
    {
      mainWindow.KeyDown += page_KeyDown;
    }
    private void page_Unloaded(object sender, RoutedEventArgs e)
    {
      mainWindow.KeyDown -= page_KeyDown;
    }

  }
}
