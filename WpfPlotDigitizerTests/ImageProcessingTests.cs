using Microsoft.VisualStudio.TestTools.UnitTesting;
using WpfPlotDigitizer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using CycWpfLibrary.Media;
using System.Windows;
using System.Drawing;

namespace WpfPlotDigitizer.Tests
{
  [TestClass()]
  public class ImageProcessingTests
  {
    [TestMethod()]
    public void GetAxisTest()
    {
      var pixelBitmapInput = new Bitmap(@"C:\Users\alex\Desktop\WPF\WpfPlotDigitizer\WpfPlotDigitizerTests\data.png").ToPixelBitmap();
      var pixelBitmapFilterW = new PixelBitmap(pixelBitmapInput.Size);
      pixelBitmapFilterW.Pixel = ImageProcessing.FilterW(pixelBitmapInput);
      var (actualAxis, axisType) = ImageProcessing.GetAxis(pixelBitmapFilterW);
      var expectedAxis = new Rect(87, 20, 747, 547);
      Assert.AreEqual(expectedAxis, actualAxis);
    }
  }
}