using CycWpfLibrary;
using CycWpfLibrary.Resource;
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
    public PageManagerBase PageManager => pageManager;
    
    /// <summary>
    /// For initializing
    /// </summary>
    public ApplicationManager ApplicationManager { get; set; } = appManager;

    /// <summary>
    /// For initializing
    /// </summary>
    public MessageManager MessageManager { get; set; } = messageManager;
  }
}
