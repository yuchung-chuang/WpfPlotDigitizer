using CycWpfLibrary;
using CycWpfLibrary.Controls;
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

    public PageManagerBase PageManager => pageManager;    
  }
}
