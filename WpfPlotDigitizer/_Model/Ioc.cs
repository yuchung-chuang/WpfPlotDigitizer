using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfPlotDigitizer
{
  public static class IoC
  {
    public static IKernel Kernel { get; private set; } = new StandardKernel();

    private static void BindViewModels()
    {
      Kernel.Bind<ApplicationViewModel>().ToConstant(new ApplicationViewModel());
      Kernel.Bind<ImageProcessingViewModel>().ToConstant(new ImageProcessingViewModel());
      Kernel.Bind<MainWindowViewModel>().ToConstant(new MainWindowViewModel());
      Kernel.Bind<BrowsePageViewModel>().ToConstant(new BrowsePageViewModel());
      Kernel.Bind<AxisPageViewModel>().ToConstant(new AxisPageViewModel());
      Kernel.Bind<FilterPageViewModel>().ToConstant(new FilterPageViewModel());
    }
    public static void SetUp()
    {
      BindViewModels();
    }

    public static T Get<T>() => Kernel.Get<T>();
  }
}
