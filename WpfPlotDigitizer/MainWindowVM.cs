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

namespace WpfPlotDigitizer
{
  public class MainWindowVM : ViewModelBase
  {
    public MainWindowVM()
    {
      QuestionCommand = new RelayCommand(Question);
    }

    public PageManagerBase PageManager => pageManager;

    public ICommand QuestionCommand { get; set; }
    private void Question()
    {
      
    }
  }
}
