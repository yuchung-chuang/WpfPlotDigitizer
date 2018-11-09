using CycWpfLibrary.Media;
using CycWpfLibrary.MVVM;
using System;
using System.Windows.Media.Imaging;

namespace WpfPlotDigitizer
{
  public class MainWindowVM : ViewModelBase<MainWindowVM>
  {
    private readonly ApplicationVM applicationVM = IoC.Get<ApplicationVM>();
    private readonly ImageProcessingVM ImageProcessingVM = IoC.Get<ImageProcessingVM>();

    public MainWindowVM()
    {
#if DEBUG
      ImageProcessingVM.PBInput = new BitmapImage(new Uri($"pack://application:,,,/images/data.png")).ToPixelBitmap();

      //applicationVM.PageManager.Index = (int)ApplicationPages.AxisLimit;
#endif      
    }
  }
}
