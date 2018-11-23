using CycWpfLibrary.Media;
using CycWpfLibrary.MVVM;
using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Windows;

namespace WpfPlotDigitizer
{
  public class ImageProcessingVM : ViewModelBase
  {

    public PixelBitmap PBFilterW { get; set; }

    public event Action OnPBInputChanged;
    private PixelBitmap _PBInput;
    public PixelBitmap PBInput
    {
      get => _PBInput;
      set
      {
        _PBInput = value;
        OnPBInputChanged?.Invoke();
      }
    }

    public Rect Axis { get; set; }
    public AxisType AxisType { get; set; }

    public PixelBitmap PBAxis { get; set; }

    public Image<Bgra, byte> ImageAxis { get; set; }
    public Image<Bgra, byte> ImageFilterRGB { get; set; }

  }
}
