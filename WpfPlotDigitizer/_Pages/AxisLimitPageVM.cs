using CycWpfLibrary.Media;
using CycWpfLibrary.MVVM;
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
  public class AxisLimitPageVM : ViewModelBase<AxisLimitPageVM>
  {
    private readonly ImageProcessingVM IPVM = IoC.Get<ImageProcessingVM>();


    public Mat MatInput { get; set; }
    public BitmapSource ImageSource => MatInput.ToBitmapSource();

    private Tesseract ocr;
    private PixelBitmap PBInput => IPVM.PBInput;
    private Rect Axis => IPVM.Axis;
    public void AutoGetAxisLimit()
    {
      ocr = IP.InitializeOcr("", "eng", OcrEngineMode.TesseractLstmCombined);

      var mat = new Mat();
      var source = PBInput.ToMat();
      var ocredText = IP.OcrImage(ocr, source, mat);
      IP.DrawCharacters(mat, ocredText);

      MatInput = mat;
    }

    



  }
}
