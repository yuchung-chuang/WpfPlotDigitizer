using CycWpfLibrary.Media;
using CycWpfLibrary.MVVM;
using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Windows;
using static WpfPlotDigitizer.DI;

namespace WpfPlotDigitizer
{
  public class ImageProcessingManager 
  {
    public PixelBitmap PBFilterW { get; set; }

    private PixelBitmap _PBInput;
    public PixelBitmap PBInput
    {
      get => _PBInput;
      set
      {
        _PBInput = value;
        AppManager.PageManager.TurnNext();
      }
    }

    public Rect Axis { get; set; }
    public AxisType AxisType { get; set; }

    public PixelBitmap PBAxis { get; set; }

    public Image<Bgra, byte> ImageAxis { get; set; }
    public Image<Bgra, byte> ImageFilterRGB { get; set; }

    public Image<Bgra, byte> ImageErase { get; set; }
  }
}
