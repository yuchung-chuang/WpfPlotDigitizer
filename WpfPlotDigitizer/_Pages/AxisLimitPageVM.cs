using CycWpfLibrary.Media;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.OCR;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using IP = WpfPlotDigitizer.ImageProcessing;

namespace WpfPlotDigitizer
{
  public class AxisLimitPageVM
  {
    private Tesseract _ocr;
    public BitmapSource ImageSource => ImageMat.Bitmap.ToBitmapSource();
    public Mat ImageMat { get; set; }

    public AxisLimitPageVM()
    {
      _ocr = IP.InitializeOcr("", "eng", OcrEngineMode.TesseractLstmCombined);
      var source = new Mat(@"C:\Users\alex\Desktop\WPF\WpfPlotDigitizer\WpfPlotDigitizer\images\ocr.png");
      ImageMat = new Mat();
      var ocredText = IP.OcrImage(_ocr, source, ImageMat);
      IP.DrawCharacters(ImageMat, ocredText);
      ImageMat.Bitmap.ToBitmapSource();
    }

    



  }
}
