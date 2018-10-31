using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfPlotDigitizer
{
  /// <summary>
  /// Provide static members of ViewModels to use in XAML.
  /// </summary>
  public class ViewModelLocator
  {
    public static ViewModelLocator Instance { get; private set; } = new ViewModelLocator();

    public static ApplicationViewModel ApplicationViewModel { get; } = IoC.Get<ApplicationViewModel>();
    public static MainWindowViewModel MainWindowViewModel { get; } = IoC.Get<MainWindowViewModel>();
    public static AxisPageViewModel AxisPageViewModel { get; } = IoC.Get<AxisPageViewModel>();
    public static FilterPageViewModel FilterPageViewModel { get; } = IoC.Get<FilterPageViewModel>();
  }
}
