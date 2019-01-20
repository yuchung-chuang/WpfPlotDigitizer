using CycWpfLibrary.Emgu;
using CycWpfLibrary.Media;
using CycWpfLibrary.MVVM;
using CycWpfLibrary.WinForm;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.OCR;
using Emgu.CV.Structure;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using static WpfPlotDigitizer.DI;
using Bitmap = System.Drawing.Bitmap;
using IP = WpfPlotDigitizer.ImageProcessing;
using Rectangle = System.Drawing.Rectangle;

namespace WpfPlotDigitizer
{
  public class AxLimPageVM : ViewModelBase
  {
    public AxLimPageVM()
    {
      //GetAxisLimitCommand = new RelayCommand(GetAxisLimit);
      EnableLimitLCommand = new RelayCommand(EnableLimitL);
      EnableLimitTCommand = new RelayCommand(EnableLimitT);
      EnableLimitRCommand = new RelayCommand(EnableLimitR);
      EnableLimitBCommand = new RelayCommand(EnableLimitB);
    }

    public PixelBitmap PBInput => imageData.PBInput;
    public BitmapSource imageSource => PBInput?.ToBitmapSource();

    public double? YMax { get; set; }
    public double? YMin { get; set; }
    public double? YLog { get; set; }
    public double? XMax { get; set; }
    public double? XMin { get; set; }
    public double? XLog { get; set; }

    public Rect AxLim => 
      (XMin == null || YMin == null || XMax == null || YMax == null) ?
          Rect.Empty :
          new Rect(new Point((double)XMin, (double)YMin), 
            new Point((double)XMax, (double)YMax));

    public bool EnableL { get; set; } = false;
    public bool EnableR { get; set; } = false;
    public bool EnableT { get; set; } = false;
    public bool EnableB { get; set; } = false;

    public ICommand EnableLimitRCommand { get; set; }
    public void EnableLimitR()
    {
      EnableR = true;
      EnableL = false;
    }
    public ICommand EnableLimitLCommand { get; set; }
    public void EnableLimitL()
    {
      EnableL = true;
      EnableR = false;
    }
    public ICommand EnableLimitTCommand { get; set; }
    public void EnableLimitT()
    {
      EnableT = true;
      EnableB = false;
    }
    public ICommand EnableLimitBCommand { get; set; }
    public void EnableLimitB()
    {
      EnableB = true;
      EnableT = false;
    }

    #region Deprecated
    public double? XMaxT { get; set; }
    public double? XMinT { get; set; }
    public double? XLogT { get; set; }
    public double? YMaxR { get; set; }
    public double? YMinR { get; set; }
    public double? YLogR { get; set; }

    public PixelBitmap PBModified { get; set; }
    public BitmapSource ImageSource => PBModified?.ToBitmapSource();

    private readonly Tesseract ocr = IP.InitializeOcr("", "eng", OcrEngineMode.TesseractOnly, "0123456789");
    private AxisType AxisType => imageData.AxisType;
    private Rect Axis => imageData.Axis;
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
        EnableL = true;
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
        EnableB = true;
        var rectangleBL = new Rect(Axis.Left - ocrLength.width / 2, Axis.Bottom, ocrLength.width, ocrLength.height / 2).ToWinForm();
        var rectangleBR = new Rect(Axis.Right - ocrLength.width / 2, Axis.Bottom, ocrLength.width, ocrLength.height / 2).ToWinForm();
        var charBL = GetLocalAxisLimit(image, rectangleBL);
        var charBR = GetLocalAxisLimit(image, rectangleBR);

      }
      if (AxisType.Right)
      {
        EnableR = true;
        var rectangleRB = new Rect(Axis.Right, Axis.Bottom - ocrLength.height / 2, ocrLength.width / 2, ocrLength.height).ToWinForm();
        var rectangleRT = new Rect(Axis.Right, Axis.Top - ocrLength.height / 2, ocrLength.width / 2, ocrLength.height).ToWinForm();
        var charRB = GetLocalAxisLimit(image, rectangleRB);
        var charRT = GetLocalAxisLimit(image, rectangleRT);
      }
      if (AxisType.Top)
      {
        EnableT = true;
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



