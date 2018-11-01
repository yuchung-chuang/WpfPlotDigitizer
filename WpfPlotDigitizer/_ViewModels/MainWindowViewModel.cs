using CycWpfLibrary.Media;
using CycWpfLibrary.MVVM;
using System;
using System.Windows.Media.Imaging;

namespace WpfPlotDigitizer
{
  public class MainWindowViewModel : ViewModelBase<MainWindowViewModel>
  {
    private static ApplicationViewModel ApplicationViewModel = IoC.Get<ApplicationViewModel>();
    private static ImageProcessingViewModel ImageProcessingViewModel = IoC.Get<ImageProcessingViewModel>();

    public MainWindowViewModel()
    {
#if DEBUG
      //ImageProcessingViewModel.pixelBitmapInput = new BitmapImage(new Uri($"pack://application:,,,/images/data.png")).ToPixelBitmap();
#endif      
    }
  }
}
