using MahApps.Metro.Controls;
using Microsoft.Win32;
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
using System.Windows.Threading;

namespace WpfPlotDigitizer
{
  /// <summary>
  /// MainWindow.xaml 的互動邏輯
  /// </summary>
  public partial class MainWindow : MetroWindow
  {
    public MainWindow()
    {
      InitializeComponent();
      DataContext = new MainWindowViewModel();
    }

    private void btnNext_Click(object sender, RoutedEventArgs e)
    {
      if (tabcontrolMain.SelectedIndex < tabcontrolMain.Items.Count - 1)
      {
        tabcontrolMain.SelectedIndex++;

      }
    }

    private void btnBack_Click(object sender, RoutedEventArgs e)
    {
      if (tabcontrolMain.SelectedIndex > 0)
      {
        tabcontrolMain.SelectedIndex--;

      }
    }
  }
}
