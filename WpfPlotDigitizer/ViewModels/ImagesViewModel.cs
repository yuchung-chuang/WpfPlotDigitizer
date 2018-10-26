using CycWpfLibrary.Media;
using CycWpfLibrary.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace WpfPlotDigitizer
{
  public class ImagesViewModel : ViewModelBase
  {
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
      }
    }
    public BitmapSource bitmapSourceInput => pixelBitmapInput?.ToBitmapSource();

    public PixelBitmap pixelBitmapFilterW { get; set; }

    public PixelBitmap pixelBitmapFilterRGB { get; set; }
    public BitmapSource bitmapSourceFilterRGB => pixelBitmapFilterRGB?.ToBitmapSource();
  }
}
