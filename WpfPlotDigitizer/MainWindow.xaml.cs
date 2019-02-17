using CycLibrary.UserControls;
using MahApps.Metro.Controls;
using System;
using System.Windows;
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

    public PageControl pageControl;

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
