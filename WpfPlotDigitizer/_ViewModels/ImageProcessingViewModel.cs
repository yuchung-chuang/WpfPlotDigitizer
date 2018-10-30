using CycWpfLibrary.Media;
using CycWpfLibrary.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace WpfPlotDigitizer
{
  public class ImageProcessingViewModel : ViewModelBase<ImageProcessingViewModel>
  {
    private readonly ApplicationViewModel applicationViewModel = IoC.Get<ApplicationViewModel>();
    private readonly AxisPageViewModel axisPageViewModel = IoC.Get<AxisPageViewModel>();

    public PixelBitmap _pixelBitmapInput { get; set; }
    public PixelBitmap pixelBitmapInput
    {
      get => _pixelBitmapInput;
      set
      {
        _pixelBitmapInput = value;
        // After update input image, automatically filterW and GetAxis
        pixelBitmapFilterW = new PixelBitmap(pixelBitmapInput.Size);
        pixelBitmapFilterW.Pixel = ImageProcessing.FilterW(pixelBitmapInput);

        pixelBitmapFilterRGB = pixelBitmapInput.Clone() as PixelBitmap;

        axisPageViewModel.AutoGetAxis();
        applicationViewModel.TurnNext();
      }
    }

    public PixelBitmap pixelBitmapFilterW { get; set; }
    public Rect Axis { get; set; }


    public PixelBitmap pixelBitmapFilterRGB { get; set; }

  }
}
