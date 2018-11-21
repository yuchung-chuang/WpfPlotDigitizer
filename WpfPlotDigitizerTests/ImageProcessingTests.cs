using Microsoft.VisualStudio.TestTools.UnitTesting;
using CycWpfLibrary.Media;
using System.Windows;
using System.Windows.Media;
using Bitmap = System.Drawing.Bitmap;
using System.Threading;
using CycWpfLibrary;
using System;
using System.Diagnostics;

namespace WpfPlotDigitizer.Tests
{
  [TestClass()]
  public class ImageProcessingTests
  {
    private PixelBitmap image;

    public ImageProcessingTests()
    {
      image = new Bitmap(@"C:\Users\alex\Desktop\WPF\WpfPlotDigitizer\WpfPlotDigitizerTests\data.png").ToPixelBitmap();
    }

    [TestMethod()]
    public void GetAxisTest()
    {
      var imageFilterW = new PixelBitmap(image.Size);
      imageFilterW.Pixel = ImageProcessing.FilterW(image);
      var (actualAxis, axisType) = ImageProcessing.GetAxis(imageFilterW);
      var expectedAxis = new Rect(87, 20, 747, 547);
      Assert.AreEqual(expectedAxis, actualAxis);
    }

    [TestMethod()]
    public void FilterRGBTest()
    {
      PixelBitmap imageFilterRGB = new PixelBitmap();
      double ms = 0;
      int n = 100;
      for (int i = 0; i < n; i++)
      {
        ms += NativeMethod.TimeIt(() => imageFilterRGB = ImageProcessing.FilterRGB(image, Color.FromRgb(250, 250, 250), Color.FromRgb(50, 50, 50), CancellationToken.None));
        if (i == 0)
        {
          ms = 0;
        }
      }
      ms /= n;
      Debug.WriteLine($"Average: {ms}");
      imageFilterRGB.ShowSnapShot();
      Assert.IsTrue(ms < 25);
      
    }
  }
}