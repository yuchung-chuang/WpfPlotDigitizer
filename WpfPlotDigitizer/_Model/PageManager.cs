using CycWpfLibrary;
using CycWpfLibrary.Controls;
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
  }
}
