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
using CycWpfLibrary.Media;

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


    public enum MouseControlState
    {
      None = 0x0000,
      Drag = 0x0001,
      Enter = 0x0004,
    }
    private double factor = 1.1;
    private MouseControlState state;

    private void image_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
    {
      image.Width *= (e.Delta > 0) ? factor : 1 / factor;
      image.Height *= (e.Delta > 0) ? factor : 1 / factor;

      e.Handled = true; // block the Parent.MouseWheel event
    }

    private void image_MouseDown(object sender, MouseButtonEventArgs e)
    {

    }

    private void image_MouseUp(object sender, MouseButtonEventArgs e)
    {

    }

    private void image_MouseMove(object sender, MouseEventArgs e)
    {

    }

    private void image_MouseEnter(object sender, MouseEventArgs e)
    {

    }

    private void image_MouseLeave(object sender, MouseEventArgs e)
    {

    }
  }
}
