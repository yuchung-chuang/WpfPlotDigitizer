using CycWpfLibrary.Controls;
using CycWpfLibrary.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WpfPlotDigitizer
{
  public enum ApplicationPages
  {
    Browse,
    Axis,
    AxisLimit,
    Filter,
    Erase,
    Save,
    /// <summary>
    /// Number of pages, should always be the last element of enum
    /// </summary>
    NumOfPages,
  }

  public class PageManager : NotifyableObject, IPageManager
  {
    public PageManager()
    {
      TurnNextCommand = new RelayCommand(TurnNext, CanTurnNext);
      TurnBackCommand = new RelayCommand(TurnBack, CanTurnBack);
      IoC.Get<IoC>().ViewModelsLoaded += OnViewModelsLoaded;
    }

    private void OnViewModelsLoaded()
    {
      browsePage    = new BrowsePage();
      axisPage      = new AxisPage();
      axisLimitPage = new AxisLimitPage();
      filterPage    = new FilterPage();
      erasePage     = new ErasePage();
      savePage      = new SavePage();
      emptyPage     = new UserControl();
    }

    // Singleton fields
    private BrowsePage    browsePage   ;
    private AxisPage      axisPage     ;
    private AxisLimitPage axisLimitPage;
    private FilterPage    filterPage   ;
    private ErasePage     erasePage    ;
    private SavePage      savePage     ;
    private UserControl   emptyPage    ;

    private UserControl GetCurrentPage()
    {
      UserControl CurrentPage = emptyPage;
      switch ((ApplicationPages)Index)
      {
        case ApplicationPages.Browse:
          CurrentPage = browsePage;
          break;
        case ApplicationPages.Axis:
          CurrentPage = axisPage;
          break;
        case ApplicationPages.AxisLimit:
          CurrentPage = axisLimitPage;
          break;
        case ApplicationPages.Filter:
          CurrentPage = filterPage;
          break;
        case ApplicationPages.Erase:
          CurrentPage = erasePage;
          break;
        case ApplicationPages.Save:
          CurrentPage = savePage;
          break;
        default:
          break;
      }
      return CurrentPage;
    }

    public int Index { get; set; } = 0;
    public UserControl CurrentPage => GetCurrentPage();

    public ICommand TurnNextCommand { get; set; }
    public void TurnBack() => Index--;
    public bool CanTurnBack() => Index > 0;

    public ICommand TurnBackCommand { get; set; }
    public event Action TurnNextEvent;
    public void TurnNext()
    {
      TurnNextEvent?.Invoke();
      Index++;
    }
    private readonly int NumOfPages = (int)ApplicationPages.NumOfPages;
    public bool CanTurnNext() => Index < NumOfPages - 1;
  }
}
