using CycWpfLibrary.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace WpfPlotDigitizer
{
  public enum ApplicationPages
  {
    Browse,
    Axis,
    Filter,
    Erase,
    /// <summary>
    /// Number of pages, should always be the last element of enum
    /// </summary>
    NumOfPages,
  }
  
  public class PageManager : ViewModelBase<PageManager>, IPageManager
  {
    public PageManager()
    {
      TurnNextCommand = new RelayCommand(TurnNext, CanTurnNext);
      TurnBackCommand = new RelayCommand(TurnBack, CanTurnBack);
    }

    public int Index { get; set; } = 0;
    public UserControl CurrentPage
    {
      get
      {
        switch ((ApplicationPages)Index)
        {
          case ApplicationPages.Browse:
            return new BrowsePage();
          case ApplicationPages.Axis:
            return new AxisPage();
          case ApplicationPages.Filter:
            return new FilterPage();
          case ApplicationPages.Erase:
            return new ErasePage();
          default:
            return new UserControl();
        }
      }
    }
    private readonly int NumOfPages = (int)ApplicationPages.NumOfPages;

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
    public bool CanTurnNext() => Index < NumOfPages - 1;
  }
}
