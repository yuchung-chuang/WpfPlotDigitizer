using CycWpfLibrary.Emgu;
using CycWpfLibrary.Media;
using CycWpfLibrary.MVVM;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.OCR;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace OcrTester
{
  public class MainWindowVM : ViewModelBase<MainWindowVM>
  {
    public BitmapSource imageSource { get; set; }

    public MainWindowVM()
    {
      var ocr = new Tesseract();
      ocr.Init("", "eng", OcrEngineMode.TesseractOnly);
      ocr.SetVariable("tessedit_char_whitelist", "0123456789");

      var image = new Image<Bgr, byte>("ocr.png");

      ocr.SetImage(image);

      if (ocr.Recognize() != 0)
        throw new Exception("Failed to recognizer image");

      var characters = ocr.GetCharacters();

      var color = Colors.Blue.ToMCvScalar();
      foreach (Tesseract.Character c in characters)
      {
        CvInvoke.Rectangle(image, c.Region, color);
        CvInvoke.PutText(image, c.Text, c.Region.Location, FontFace.HersheyPlain, 1, color);
      }

      imageSource = image.Bitmap.ToBitmapSource();
    }
  }
}
