using CycWpfLibrary.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfPlotDigitizer
{
  public class ApplicationViewModel : ViewModelBase
  {
    public ApplicationPages CurrentPage { get; set; } = ApplicationPages.Browse;
  }
}
