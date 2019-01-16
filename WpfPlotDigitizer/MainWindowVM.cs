using CycWpfLibrary.Controls;
using CycWpfLibrary.MVVM;
using static WpfPlotDigitizer.Singletons;

namespace WpfPlotDigitizer
{
  public class MainWindowVM : ViewModelBase
  {
    public PageManagerBase PageManager => appManager.PageManager;
  }
}
