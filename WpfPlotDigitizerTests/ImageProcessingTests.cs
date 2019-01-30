using WpfPlotDigitizer;
using CycWpfLibrary;
using CycWpfLibrary.Media;
using Emgu.CV;
using Emgu.CV.Structure;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using Bitmap = System.Drawing.Bitmap;
using CycWpfLibrary.Emgu;

namespace WpfPlotDigitizer.Tests
{
  [TestClass()]
  public class ImageProcessingTests
  {
    private PixelBitmap pixelBitmap;
    private Image<Bgra, byte> imageBGRA; 
    private Image<Rgba, byte> imageRGBA; 
    private Mat mat;

    public ImageProcessingTests()
    {
      pixelBitmap = new Bitmap(@"C:\Users\alex\Desktop\WPF\WpfPlotDigitizer\WpfPlotDigitizerTests\data.png").ToPixelBitmap();
      imageBGRA = pixelBitmap.ToImage<Bgra, byte>();  
      mat = imageBGRA.Mat;
    }

    [TestMethod()]
    public void GetAxisTest()
    {
      var imageFilterW = new PixelBitmap(pixelBitmap.Size)
      {
        Pixel = ImageProcessing.FilterW(pixelBitmap)
      };
      var (actualAxis, axisType) = ImageProcessing.GetAxis(imageFilterW);
      var expectedAxis = new Rect(87, 20, 747, 547);
      Assert.AreEqual(expectedAxis, actualAxis);
    }

    [Obsolete]
    public void FilterRGBTest()
    {
      PixelBitmap PBFilterRGB = new PixelBitmap();
      double ms = 0;
      int n = 100;
      for (int i = 0; i < n; i++)
      {
        ms += NativeMethod.TimeIt(() => PBFilterRGB = ImageProcessing.FilterRGB(pixelBitmap, Color.FromRgb(250, 250, 250), Color.FromRgb(10, 10, 10), CancellationToken.None));
        if (i == 0)
        {
          ms = 0;
        }
      }
      ms /= n;
      Debug.WriteLine($"Average: {ms}");
      //PBFilterRGB.Show();
      Assert.IsTrue(ms < 10);

    }

    [Obsolete]
    public void FilterRGB_EmguTest()
    {
      Image<Rgba, byte> imageFilterRGB = new Image<Rgba, byte>(imageRGBA.Size);
      double ms = 0;
      int n = 100;
      for (int i = 0; i < n; i++)
      {
        ms += NativeMethod.TimeIt(() =>
        {
          imageFilterRGB = ImageProcessing.FilterRGB_Emgu(imageRGBA, Color.FromRgb(250, 250, 250), Color.FromRgb(10, 10, 10), CancellationToken.None);
          if (i == 0)
          {
            ms = 0;
          }
        });
      }
      ms /= n;
      Debug.WriteLine($"Average: {ms}");
      //imageFilterRGB.ToPixelBitmap().Show();
      Assert.IsTrue(ms < 25);
    }

    [TestMethod()]
    public void InRangeTest()
    {
      Image<Bgra, byte> imageFilterRGB = new Image<Bgra, byte>(imageBGRA.Size);
      double ms = 0;
      int n = 100;
      for (int i = 0; i < n; i++)
      {
        ms += NativeMethod.TimeIt(() =>
        {
          imageFilterRGB = ImageProcessing.InRange(imageBGRA, Color.FromRgb(250, 250, 250), Color.FromRgb(10, 10, 10));
          if (i == 0)
          {
            ms = 0;
          }
        });
      }
      ms /= n;
      Debug.WriteLine($"Average: {ms}");
      //imageFilterRGB.ToPixelBitmap().Show();
      Assert.IsTrue(ms < 10);
    }
    
  }
}