using CycWpfLibrary;
using CycWpfLibrary.Controls;
using CycWpfLibrary.MVVM;
using System.Windows.Controls;
using static WpfPlotDigitizer.DI;

namespace WpfPlotDigitizer
{
  public enum ApplicationPages
  {
    Browse,
    AxLim,
    Axis,
    Filter,
    Erase,
    Data,
    Save,
    /// <summary>
    /// Number of pages, should always be the last element of enum
    /// </summary>
    NumOfPages,
  }

  public class PageManager : PageManagerBase
  {
    public PageManager()
    {
      TurnNextCommand = new RelayCommand(TurnNext, CanTurnNext);
      TurnBackCommand = new RelayCommand(TurnBack, CanTurnBack);
    }

    private UserControl GetCurrentPage()
    {
      UserControl CurrentPage = emptyPage;
      switch ((ApplicationPages)Index)
      {
        case ApplicationPages.Browse:
          CurrentPage = browsePage;
          break;
        case ApplicationPages.AxLim:
          CurrentPage = axLimPage;
          break;
        case ApplicationPages.Axis:
          CurrentPage = axisPage;
          break;
        case ApplicationPages.Filter:
          CurrentPage = filterPage;
          break;
        case ApplicationPages.Erase:
          CurrentPage = erasePage;
          break;
        case ApplicationPages.Data:
          CurrentPage = dataPage;
          break;
        case ApplicationPages.Save:
          CurrentPage = savePage;
          break;
        default:
          break;
      }
      return CurrentPage;
    }

    private int index;
    public override int Index
    {
      get => index;
      set
      {
        index = value;
        OnPropertyChanged(nameof(CurrentPage));
      }
    }
    public override UserControl CurrentPage => GetCurrentPage();

    private readonly int NumOfPages = (int)ApplicationPages.NumOfPages;
    public override bool CanTurnNext() => Index < NumOfPages - 1;
  }
}
