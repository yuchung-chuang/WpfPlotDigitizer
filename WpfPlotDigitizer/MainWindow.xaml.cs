using CycWpfLibrary;
using CycWpfLibrary.CustomControls;
using CycWpfLibrary.Resource;
using CycWpfLibrary.UserControls;
using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Shapes;
using static WpfPlotDigitizer.DI;

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
      DataContext = mainWindowVM;
    }
    public PageControl PageControl { get; set; }

    public void PageControl_PageAnimated(object sender, EventArgs e)
    {
      tutorialManager.CheckFirstVisitPage();
    }

    private void Tutorial_Click(object sender, RoutedEventArgs e)
    {
      tutorialManager.Tutorial();
    }

    private void About_Click(object sender, RoutedEventArgs e)
    {
      new AboutPopup().ShowDialog();
    }

    private void Lang_Click(object sender, RoutedEventArgs e)
    {
      new LangPopup().ShowDialog();
    }
  }
}
