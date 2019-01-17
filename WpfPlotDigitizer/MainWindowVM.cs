using CycWpfLibrary.Controls;
using CycWpfLibrary.MVVM;
using static WpfPlotDigitizer.DI;

namespace WpfPlotDigitizer
{
  public class MainWindowVM : ViewModelBase
  {
    public PageManagerBase PageManager => appManager.PageManager;
  }
}
