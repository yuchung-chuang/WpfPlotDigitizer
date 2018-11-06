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
  public class ImageProcessingVM : ViewModelBase<ImageProcessingVM>
  {

    public PixelBitmap pixelBitmapFilterW { get; set; }

    public event Action OnPixelBitampInputChanged;
    private PixelBitmap _pixelBitmapInput;
    public PixelBitmap pixelBitmapInput
    {
      get => _pixelBitmapInput;
      set
      {
        _pixelBitmapInput = value;
        OnPixelBitampInputChanged?.Invoke();
      }
    }


    public Rect Axis { get; set; }

    public PixelBitmap pixelBitmapFilterRGB { get; set; }

  }
}
