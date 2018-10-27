using CycWpfLibrary.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WpfPlotDigitizer
{
  public class ApplicationViewModel : ViewModelBase
  {

    public ApplicationViewModel()
    {
      TurnBackCommand = new RelayCommand(TurnBack, CanTurnBack);
      TurnNextCommand = new RelayCommand(TurnNext, CanTurnNext);
    }
    public ApplicationPages CurrentPage { get; set; } = ApplicationPages.Browse;

    public ICommand TurnBackCommand { get; set; }
    public void TurnBack()
    {
      CurrentPage--;
    }
    public bool CanTurnBack() => CurrentPage > 0;

    public ICommand TurnNextCommand { get; set; }
    public void TurnNext()
    {
      CurrentPage++;
    }
    public bool CanTurnNext() => CurrentPage < ApplicationPages.NumOfPages - 1;
  }
}
