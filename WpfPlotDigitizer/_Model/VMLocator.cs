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
  public class VMLocator
  {
    public static VMLocator Instance { get; private set; } = new VMLocator();

    public static ApplicationVM ApplicationVM { get; } = IoC.Get<ApplicationVM>();
    public static MainWindowVM MainWindowVM { get; } = IoC.Get<MainWindowVM>();

    public static BrowsePageVM BrowsePageVM { get; } = IoC.Get<BrowsePageVM>();
    public static AxisPageVM AxisPageVM { get; } = IoC.Get<AxisPageVM>();
    public static AxisLimitPageVM AxisLimitPageVM { get; } = IoC.Get<AxisLimitPageVM>();
    public static FilterPageVM FilterPageVM { get; } = IoC.Get<FilterPageVM>();
    public static SavePageVM SavePageVM { get; } = IoC.Get<SavePageVM>();
  }
}
