using CycLibrary.Emgu;
using CycLibrary.Media;
using CycLibrary.MVVM;
using CycLibrary.WinForm;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.OCR;
using Emgu.CV.Structure;
using System;
using System.ComponentModel;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using static WpfPlotDigitizer.DI;
using IP = WpfPlotDigitizer.ImageProcessing;
using Rectangle = System.Drawing.Rectangle;

namespace WpfPlotDigitizer
{
  public class AxLimPageVM : ViewModelBase
  {
    public AxLimPageVM()
    {
    }

    public PixelBitmap PBInput => appData.PBInput;
    public BitmapSource imageSource => PBInput?.ToBitmapSource();

    public bool IsValid => IsViewValid && IsDataValid;
    public bool IsViewValid { get; set; } = true;
    public bool IsDataValid => XMin != null && XMax != null && YMin != null && YMax != null &&
      XMin <= XMax && YMin <= YMax;

    public double? XMax { get; set; }
    public double? XMin { get; set; }
    public double? YMax { get; set; }
    public double? YMin { get; set; }
    public Rect AxLim
    {
      get => new Rect(new Point(XMin ?? 0, YMin ?? 0),
            new Point(XMax ?? 1, YMax ?? 1));
      set
      {
        XMax = value.Right;
        XMin = value.Left;
        YMax = value.Bottom;
        YMin = value.Top;
      }
    }

    public double? XLog { get; set; }
    public double? YLog { get; set; }
    public Point AxLogBase
    {
      get => new Point(XLog ?? -1, YLog ?? -1);
      set
      {
        XLog = value.X;
        YLog = value.Y;
      }
    }

  }
}



