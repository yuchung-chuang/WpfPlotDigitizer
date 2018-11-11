using CycWpfLibrary.Emgu;
using CycWpfLibrary.Media;
using CycWpfLibrary.MVVM;
using CycWpfLibrary.WinForm;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.OCR;
using Emgu.CV.Structure;
using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Bitmap = System.Drawing.Bitmap;
using IP = WpfPlotDigitizer.ImageProcessing;
using Rectangle = System.Drawing.Rectangle;

namespace WpfPlotDigitizer
{
  public class AxisLimitPageVM : ViewModelBase<AxisLimitPageVM>
  {
    public AxisLimitPageVM()
    {
      GetAxisLimitCommand = new RelayCommand(GetAxisLimit);
      EnableLimitLCommand = new RelayCommand(EnableLimitL);
      EnableLimitTCommand = new RelayCommand(EnableLimitT);
      EnableLimitRCommand = new RelayCommand(EnableLimitR);
      EnableLimitBCommand = new RelayCommand(EnableLimitB);
      IoC.Get<IoC>().ViewModelsLoaded += OnViewModelsLoaded;
    }

    private void OnViewModelsLoaded()
    {
      IPVM = IoC.Get<ImageProcessingVM>();
    }
    //Singleton fields
    private ImageProcessingVM IPVM;

    public PixelBitmap PBModified { get; set; }
    public BitmapSource ImageSource => PBModified?.ToBitmapSource();

    public double? YMaxL { get; set; }
    public double? YMinL { get; set; }
    public double? YLogL { get; set; }
    public double? XMaxT { get; set; }
    public double? XMinT { get; set; }
    public double? XLogT { get; set; }
    public double? YMaxR { get; set; }
    public double? YMinR { get; set; }
    public double? YLogR { get; set; }
    public double? XMaxB { get; set; }
    public double? XMinB { get; set; }
    public double? XLogB { get; set; }

    public bool EnableL { get; set; } = false;
    public bool EnableR { get; set; } = false;
    public bool EnableT { get; set; } = false;
    public bool EnableB { get; set; } = false;

    private readonly Tesseract ocr = IP.InitializeOcr("", "eng", OcrEngineMode.TesseractOnly, "0123456789");
    private PixelBitmap PBInput => IPVM.PBInput;
    private AxisType AxisType => IPVM.AxisType;
    private Rect Axis => IPVM.Axis;
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
        var textLT = GetLocalAxisLimit(image, rectangleLT);
        var textLB = GetLocalAxisLimit(image, rectangleLB);
      }
      if (AxisType.Bottom)
      {
        EnableB = true;
        var rectangleBL = new Rect(Axis.Left - ocrLength.width / 2, Axis.Bottom, ocrLength.width, ocrLength.height / 2).ToWinForm();
        var rectangleBR = new Rect(Axis.Right - ocrLength.width / 2, Axis.Bottom, ocrLength.width, ocrLength.height / 2).ToWinForm();
        var textBL = GetLocalAxisLimit(image, rectangleBL);
        var textBR = GetLocalAxisLimit(image, rectangleBR);

      }
      if (AxisType.Right)
      {
        EnableR = true;
        var rectangleRB = new Rect(Axis.Right, Axis.Bottom - ocrLength.height / 2, ocrLength.width / 2, ocrLength.height).ToWinForm();
        var rectangleRT = new Rect(Axis.Right, Axis.Top - ocrLength.height / 2, ocrLength.width / 2, ocrLength.height).ToWinForm();
        var textRB = GetLocalAxisLimit(image, rectangleRB);
        var textRT = GetLocalAxisLimit(image, rectangleRT);
      }
      if (AxisType.Top)
      {
        EnableT = true;
        var rectangleTR = new Rect(Axis.Right - ocrLength.width / 2, Axis.Top - ocrLength.height / 2, ocrLength.width, ocrLength.height / 2).ToWinForm();
        var rectangleTL = new Rect(Axis.Left - ocrLength.width / 2, Axis.Top - ocrLength.height / 2, ocrLength.width, ocrLength.height / 2).ToWinForm();
        var textTR = GetLocalAxisLimit(image, rectangleTR);
        var textTL = GetLocalAxisLimit(image, rectangleTL);
      }

      PBModified = image.Bitmap.ToPixelBitmap();
    }

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
  }
}



