using CycWpfLibrary;
using CycWpfLibrary.MVVM;
using CycWpfLibrary.Resources;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using static WpfPlotDigitizer.DI;
using MahApps.Metro;
using CycWpfLibrary.CustomControls;

namespace WpfPlotDigitizer
{
  public class MainWindowVM : ViewModelBase
  {
    public MainWindowVM()
    {

    }
    /// <summary>
    /// 必須透過<see cref="ApplicationManager"/>來取得<see cref="PageManager"/>，否則會無法實例化<see cref="ApplicationManager"/>!
    /// </summary>
    public PageManagerBase PageManager => appManager.PageManager; 
  }
}
