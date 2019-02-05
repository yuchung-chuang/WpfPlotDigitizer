using CycWpfLibrary.Emgu;
using CycWpfLibrary.Media;
using CycWpfLibrary.MVVM;
using CycWpfLibrary.WinForm;
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

    #region Deprecated
    public double? XMaxT { get; set; }
    public double? XMinT { get; set; }
    public double? XLogT { get; set; }
    public double? YMaxR { get; set; }
    public double? YMinR { get; set; }
    public double? YLogR { get; set; }

    [Obsolete]
    public PixelBitmap PBModified { get; set; }
    [Obsolete]
    public BitmapSource ImageSource => PBModified?.ToBitmapSource();

    private readonly Tesseract ocr = IP.InitializeOcr("", "eng", OcrEngineMode.TesseractOnly, "0123456789");
    private AxisType AxisType => appData.AxisType;
    private Rect Axis => appData.Axis;
    private (double width, double height) ocrLength => (Axis.Width / 5, Axis.Height / 5);
    private Tesseract.Character[] GetLocalAxisLimit(Image<Bgr, byte> image, Rectangle rectangle)
    {
      image.ROI = rectangle;
      var ocredText = IP.OcrImage(ocr, image);
      IP.DrawCharacters(image, ocredText);
      image.ClearROI();
      CvInvoke.Rectangle(image, rectangle, Colors.Red.ToMCvScalar());
      return ocredText;
    }
    public ICommand GetAxisLimitCommand { get; set; }
    [Obsolete]
    public void GetAxisLimit()
    {

      var image = new Image<Bgr, byte>(PBInput.Bitmap);
      Mat imgGrey = new Mat();
      CvInvoke.CvtColor(image, imgGrey, ColorConversion.Bgr2Gray);
      Mat imgThresholded = new Mat();
      CvInvoke.Threshold(imgGrey, imgThresholded, 190, 255, ThresholdType.Binary);
      image = imgThresholded.ToImage<Bgr, byte>();

      if (AxisType.Left)
      {
        var rectangleLT = new Rect(Axis.Left - ocrLength.width / 2, Axis.Top - ocrLength.height / 2, ocrLength.width / 2, ocrLength.height).ToWinForm();
        var rectangleLB = new Rect(Axis.Left - ocrLength.width / 2, Axis.Bottom - ocrLength.height / 2, ocrLength.width / 2, ocrLength.height).ToWinForm();
        var charLT = GetLocalAxisLimit(image, rectangleLT);
        var charLB = GetLocalAxisLimit(image, rectangleLB);

        //var distances = charLT.Select(c => Point.Subtract(c.Region.Location.ToWpf(), Axis.Location).Length);
        //var minDistance = distances.Min();
        //var minIndex = distances.ToList().IndexOf(minDistance);
        // Align the text and find the nearest one!!
      }
      if (AxisType.Bottom)
      {
        var rectangleBL = new Rect(Axis.Left - ocrLength.width / 2, Axis.Bottom, ocrLength.width, ocrLength.height / 2).ToWinForm();
        var rectangleBR = new Rect(Axis.Right - ocrLength.width / 2, Axis.Bottom, ocrLength.width, ocrLength.height / 2).ToWinForm();
        var charBL = GetLocalAxisLimit(image, rectangleBL);
        var charBR = GetLocalAxisLimit(image, rectangleBR);

      }
      if (AxisType.Right)
      {
        var rectangleRB = new Rect(Axis.Right, Axis.Bottom - ocrLength.height / 2, ocrLength.width / 2, ocrLength.height).ToWinForm();
        var rectangleRT = new Rect(Axis.Right, Axis.Top - ocrLength.height / 2, ocrLength.width / 2, ocrLength.height).ToWinForm();
        var charRB = GetLocalAxisLimit(image, rectangleRB);
        var charRT = GetLocalAxisLimit(image, rectangleRT);
      }
      if (AxisType.Top)
      {
        var rectangleTR = new Rect(Axis.Right - ocrLength.width / 2, Axis.Top - ocrLength.height / 2, ocrLength.width, ocrLength.height / 2).ToWinForm();
        var rectangleTL = new Rect(Axis.Left - ocrLength.width / 2, Axis.Top - ocrLength.height / 2, ocrLength.width, ocrLength.height / 2).ToWinForm();
        var charTR = GetLocalAxisLimit(image, rectangleTR);
        var charTL = GetLocalAxisLimit(image, rectangleTL);
      }

      PBModified = image.Bitmap.ToPixelBitmap();
    }
    #endregion
  }
}



