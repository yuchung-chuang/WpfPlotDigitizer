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
      DataContext = new MainWindowViewModel
      {
        tabControlMain = tabcontrolMain,
        imageAxis = imageAxis,
      };


    }

    private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
    {
      
    }

    private void imageAxis_Loaded(object sender, RoutedEventArgs e)
    {
      var shiftLeft = (canvasAxis.ActualWidth - imageAxis.ActualWidth) / 2;
      var shiftTop = (canvasAxis.ActualHeight - imageAxis.ActualHeight) / 2;
      (borderImageAxis.RenderTransform as TranslateTransform).X = shiftLeft;
      (borderImageAxis.RenderTransform as TranslateTransform).Y = shiftTop;
      (gridRect.RenderTransform as TranslateTransform).X = shiftLeft;
      (gridRect.RenderTransform as TranslateTransform).Y = shiftTop;
    }

    private void gridRect_Loaded(object sender, RoutedEventArgs e)
    {
      
    }
  }
}
