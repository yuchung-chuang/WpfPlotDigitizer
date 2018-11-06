using CycWpfLibrary.Media;
using CycWpfLibrary.MVVM;
using System;
using System.Windows.Media.Imaging;

namespace WpfPlotDigitizer
{
  public class MainWindowVM : ViewModelBase<MainWindowVM>
  {
    private readonly ImageProcessingVM ImageProcessingVM = IoC.Get<ImageProcessingVM>();

    public MainWindowVM()
    {
#if DEBUG
      ImageProcessingVM.pixelBitmapInput = new BitmapImage(new Uri($"pack://application:,,,/images/data.png")).ToPixelBitmap();
#endif      
    }
  }
}
