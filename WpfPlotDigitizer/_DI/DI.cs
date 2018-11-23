using Dna;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace WpfPlotDigitizer
{
  public static class DI
  {
    public static ApplicationManager AppManager
      => Framework.Service<ApplicationManager>();
    public static ImageProcessingManager IPManager
      => Framework.Service<ImageProcessingManager>();

    public static readonly MainWindowVM mainWindowVM = new MainWindowVM();
    public static readonly BrowsePageVM browsePageVM = new BrowsePageVM();
    public static readonly AxisPageVM axisPageVM = new AxisPageVM();
    public static readonly AxLimPageVM axisLimitPageVM = new AxLimPageVM();
    public static readonly FilterPageVM  filterPageVM= new FilterPageVM();

    public static readonly BrowsePage browsePage = new BrowsePage();
    public static readonly AxisPage axisPage = new AxisPage();
    public static readonly AxisLimitPage axisLimitPage = new AxisLimitPage();
    public static readonly FilterPage filterPage = new FilterPage();
    public static readonly ErasePage erasePage = new ErasePage();
    public static readonly SavePage savePage = new SavePage();
    public static readonly UserControl emptyPage = new UserControl();
  }
}
