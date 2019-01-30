using CycWpfLibrary.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WpfPlotDigitizer
{
  public class SplashPageVM : ViewModelBase
  {
    public SplashPageVM()
    {
      CompleteCommand = new RelayCommand(Complete);
    }

    public ICommand CompleteCommand { set; get; }

    public event EventHandler CompleteEvent;

    private void Complete()
    {
      CompleteEvent?.Invoke(this, null);
    }
  }
}
