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

    public PixelBitmap pixelBitmapFilterW { get; set; }
    private PixelBitmap _pixelBitmapInput;
    public PixelBitmap pixelBitmapInput
    {
      get => _pixelBitmapInput;
      set
      {
        _pixelBitmapInput = value;
        // After update input image, automatically filterW and GetAxis
        pixelBitmapFilterW = new PixelBitmap(pixelBitmapInput.Size);
        pixelBitmapFilterW.Pixel = ImageProcessing.FilterW(pixelBitmapInput);

        axisPageViewModel.AutoGetAxis();
        applicationViewModel.TurnNext();
      }
    }

    private Rect _Axis;
    public Rect Axis
    {
      get => _Axis;
      set
      {
        _Axis = value;
        var bitmap = pixelBitmapInput.Bitmap.Crop(_Axis);
        pixelBitmapFilterRGB = new PixelBitmap(bitmap);
      }
    }


    public PixelBitmap pixelBitmapFilterRGB { get; set; }

  }
}
