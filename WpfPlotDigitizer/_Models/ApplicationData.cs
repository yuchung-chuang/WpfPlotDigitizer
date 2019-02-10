using CycWpfLibrary.Media;
using CycWpfLibrary.MVVM;
using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Windows;
using static WpfPlotDigitizer.DI;

namespace WpfPlotDigitizer
{
  public class ApplicationData : ObservableObject
  {
    //Browse
    public PixelBitmap PBInput { get; set; }
    //AxLim
    public Rect AxLim { get; set; }
    public Point AxLogBase { get; set; }
    //Axis
    public PixelBitmap PBFilterW { get; set; }
    public Rect Axis { get; set; }
    public AxisType AxisType { get; set; }
    public PixelBitmap PBAxis { get; set; }
    public Image<Bgra, byte> ImageAxis { get; set; }
    //Filter
    public Image<Bgra, byte> ImageFilterRGB { get; set; }
    //Erase
    public Image<Bgra, byte> ImageErase { get; set; }
    //Data
    public List<Point> Data { get; set; }
    public Image<Bgra, byte> ImageData { get; set; }
    //Save 
    public Image<Bgra, byte> ImageSave { get; set; }
  }
}
