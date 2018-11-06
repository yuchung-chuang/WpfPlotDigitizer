using CycWpfLibrary.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WpfPlotDigitizer
{
  public class ApplicationViewModel : ViewModelBase<ApplicationViewModel>
  {
    public IPageManager PageManager { get; } = IoC.Get<PageManager>();

  }
}
