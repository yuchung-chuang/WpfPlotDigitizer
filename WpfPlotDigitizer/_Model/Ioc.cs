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
      Kernel.Bind<ImageProcessingVM>().ToConstant(new ImageProcessingVM());
      Kernel.Bind<PageManager>().ToConstant(new PageManager());
      Kernel.Bind<AxisPageVM>().ToConstant(new AxisPageVM());
      Kernel.Bind<FilterPageVM>().ToConstant(new FilterPageVM());
      Kernel.Bind<ApplicationVM>().ToConstant(new ApplicationVM());
      Kernel.Bind<MainWindowVM>().ToConstant(new MainWindowVM());
    }
    public static void SetUp()
    {
      BindViewModels();
    }

    public static T Get<T>() => Kernel.Get<T>();
  }
}
