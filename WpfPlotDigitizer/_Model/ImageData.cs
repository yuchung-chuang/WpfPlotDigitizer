using CycWpfLibrary.Media;
using CycWpfLibrary.MVVM;
using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Windows;
using static WpfPlotDigitizer.DI;

namespace WpfPlotDigitizer
{
  public class ImageData
  {
    public PixelBitmap PBFilterW { get; set; }
    public PixelBitmap PBInput { get; set; }

    public Rect Axis { get; set; }
    public AxisType AxisType { get; set; }

    public PixelBitmap PBAxis { get; set; }

    public Image<Bgra, byte> ImageAxis { get; set; }
    public Image<Bgra, byte> ImageFilterRGB { get; set; }

    public Image<Bgra, byte> ImageErase { get; set; }
  }
}
