using Ninject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfPlotDigitizer
{
  public class IoC
  {
    public static IKernel Kernel { get; private set; } = new StandardKernel();

    public event Action ViewModelsLoaded;

    public static void SetUp()
    {
      Kernel.Bind<IoC>().ToConstant(new IoC());
      Kernel.Bind<ImageProcessingVM>().ToConstant(new ImageProcessingVM());
      Kernel.Bind<PageManager>().ToConstant(new PageManager());

      Kernel.Bind<BrowsePageVM>().ToConstant(new BrowsePageVM());
      Kernel.Bind<AxisPageVM>().ToConstant(new AxisPageVM());
      Kernel.Bind<AxisLimitPageVM>().ToConstant(new AxisLimitPageVM());
      Kernel.Bind<FilterPageVM>().ToConstant(new FilterPageVM());
      Kernel.Bind<ErasePageVM>().ToConstant(new ErasePageVM());
      Kernel.Bind<SavePageVM>().ToConstant(new SavePageVM());

      Kernel.Bind<ApplicationVM>().ToConstant(new ApplicationVM());
      Kernel.Bind<MainWindowVM>().ToConstant(new MainWindowVM());

      Get<IoC>().ViewModelsLoaded?.Invoke();
    }
    

    /// <summary>
    /// 注意! 若在尚未Bind時呼叫Get，系統會自動new一個實例！
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T Get<T>() => Kernel.Get<T>();
  }
}
