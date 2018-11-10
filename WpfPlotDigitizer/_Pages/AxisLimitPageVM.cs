using CycWpfLibrary.Emgu;
using CycWpfLibrary.Media;
using CycWpfLibrary.MVVM;
using CycWpfLibrary.WinForm;
using Emgu.CV;
using Emgu.CV.OCR;
using Emgu.CV.Structure;
using System;
using Rectangle = System.Drawing.Rectangle;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using IP = WpfPlotDigitizer.ImageProcessing;
using Emgu.CV.CvEnum;

namespace WpfPlotDigitizer
{
  public class AxisLimitPageVM : ViewModelBase<AxisLimitPageVM>
  {
    private readonly ImageProcessingVM IPVM = IoC.Get<ImageProcessingVM>();

    public System.Drawing.Bitmap BitmapInput { get; set; }
    public BitmapSource ImageSource => BitmapInput.ToBitmapSource();

    private readonly Tesseract ocr = IP.InitializeOcr("", "eng", OcrEngineMode.TesseractOnly, "0123456789");
    private PixelBitmap PBInput => IPVM.PBInput;
    private Rect Axis => IPVM.Axis;
    private (double width, double height) ocrLength => (Axis.Width / 5, Axis.Height / 5);
    public void AutoGetAxisLimit()
    {
      var image = new Image<Bgr, byte>(PBInput.Bitmap);
      Mat imgGrey = new Mat();
      CvInvoke.CvtColor(image, imgGrey, ColorConversion.Bgr2Gray);
      Mat imgThresholded = new Mat();
      CvInvoke.Threshold(imgGrey, imgThresholded, 190, 255, ThresholdType.Binary);
      image = imgThresholded.ToImage<Bgr, byte>();

      var rectangleLT = new Rect(Axis.Left - ocrLength.width / 2, Axis.Top - ocrLength.height / 2, ocrLength.width / 2, ocrLength.height).ToWinForm();
      var rectangleLB = new Rect(Axis.Left - ocrLength.width / 2, Axis.Bottom - ocrLength.height / 2, ocrLength.width / 2, ocrLength.height).ToWinForm();
      var textLT = GetLocalAxisLimit(image, rectangleLT);
      var textLB = GetLocalAxisLimit(image, rectangleLB);

      var rectangleBL = new Rect(Axis.Left - ocrLength.width / 2, Axis.Bottom, ocrLength.width, ocrLength.height / 2).ToWinForm();
      var rectangleBR = new Rect(Axis.Right - ocrLength.width / 2, Axis.Bottom, ocrLength.width, ocrLength.height / 2).ToWinForm();
      var textBL = GetLocalAxisLimit(image, rectangleBL);
      var textBR = GetLocalAxisLimit(image, rectangleBR);

      var rectangleRB = new Rect(Axis.Right, Axis.Bottom - ocrLength.height / 2, ocrLength.width / 2, ocrLength.height).ToWinForm();
      var rectangleRT = new Rect(Axis.Right, Axis.Top - ocrLength.height / 2, ocrLength.width / 2, ocrLength.height).ToWinForm();
      var textRB = GetLocalAxisLimit(image, rectangleRB);
      var textRT = GetLocalAxisLimit(image, rectangleRT);

      var rectangleTR = new Rect(Axis.Right - ocrLength.width / 2, Axis.Top - ocrLength.height / 2, ocrLength.width, ocrLength.height / 2).ToWinForm();
      var rectangleTL = new Rect(Axis.Left - ocrLength.width / 2, Axis.Top - ocrLength.height / 2, ocrLength.width, ocrLength.height / 2).ToWinForm();
      var textTR = GetLocalAxisLimit(image, rectangleTR);
      var textTL = GetLocalAxisLimit(image, rectangleTL);

      BitmapInput = image.Bitmap;
    }
    private Tesseract.Character[] GetLocalAxisLimit(Image<Bgr, byte> image, Rectangle rectangle)
    {
      image.ROI = rectangle;
      var ocredText = IP.OcrImage(ocr, image);
      IP.DrawCharacters(image, ocredText);
      image.ClearROI();
      CvInvoke.Rectangle(image, rectangle, Colors.Red.ToMCvScalar());
      return ocredText;
    }
  }
}
