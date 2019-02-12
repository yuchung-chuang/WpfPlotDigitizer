using CycWpfLibrary;
using CycWpfLibrary.Media;
using CycWpfLibrary.MVVM;
using System.Windows;
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
      TurnNextCommand = new RelayCommand<object, bool>(TurnNext, CanTurnNext);
      TurnBackCommand = new RelayCommand<object, bool>(TurnBack, CanTurnBack);
    }

    /// <summary>
    /// Should be consistent with <see cref="ApplicationPages"/>.
    /// </summary>
    private static readonly FrameworkElement[] allPages = new FrameworkElement[]
    {
      browsePage,
      axLimPage,
      axisPage,
      filterPage,
      erasePage,
      dataPage,
      savePage,
    };

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

    public override FrameworkElement CurrentPage => allPages[Index];
    public FrameworkElement NextPage => allPages[Index + 1];
    public FrameworkElement PreviousPage => allPages[Index - 1];

    private readonly int NumOfPages = (int)ApplicationPages.NumOfPages;
    public override bool CanTurnNext(object param = null) => Index < NumOfPages - 1;

    public override bool TurnNextValidation()
    {
      switch ((ApplicationPages)index) // From current index
      {
        case ApplicationPages.Browse:
          if (!PBInputCheck())
          {
            MessageBoxManager.Warning("Please select an image.");
            return false;
          }
          break;

          bool PBInputCheck() => appData.PBInput != null;
        case ApplicationPages.AxLim:
          if (!axLimPageVM.IsValid)
          {
            MessageBoxManager.Warning("Please type in all valid axis limits.");
            return false;
          }
          break;
        default:
          break;
      }
      return true;
    }
  }
}
