using CycLibrary;
using static WpfPlotDigitizer.DI;

namespace WpfPlotDigitizer
{
  public class MainWindowVM : ViewModelBase
  {
    public PageManagerBase PageManager => pageManager;
    
    /// <summary>
    /// For initializing
    /// </summary>
    public ApplicationManager ApplicationManager = appManager;

    /// <summary>
    /// For initializing
    /// </summary>
    public MessageManager MessageManager = messageManager;
  }
}
