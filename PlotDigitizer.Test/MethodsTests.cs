using PlotDigitizer.Core;
using Emgu.CV;
using Emgu.CV.Structure;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using PlotDigitizer.WPF;

using System;
using System.Drawing;
using System.Windows.Media.Imaging;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using System.Threading;

namespace PlotDigitizer.Core.Tests
{
    [TestClass()]
    public class MethodsTests
    {
        private EmguCvService imageService;
        private Application app;

        [TestInitialize]
        public void OnTestInitialize()
        {
            if (!UriParser.IsKnownScheme("pack"))
                app = new Application();
            imageService = new EmguCvService();
        }

        [DataTestMethod()]
        [DataRow("Assets/Hysteresis-loop-of-cobalt-ferrite-samples-CF-CF300_30-and-CF600_180.png", 91, 13, 745, 551)]
        [DataRow("Assets/data.png", 85.5, 18.5, 748.5, 548.5)]
        [DataRow("Assets/Graph-1.jpg", 43, 43, 574, 329)]
        [DataRow("Assets/Inseam-v-Height-Graph.jpg", 51, 7, 641, 418)]
        [DataRow("Assets/rnaseqdedemo_19.png", 63, 31, 377, 342)]
        [DataRow("Assets/scatter_and_hist_border.png", 59.5, 222.5, 468.5, 468.5)]
        [DataRow("Assets/Screenshot 2021-06-26 230901.png", 106, 18, 835, 643)]
        public void GetAxisLocationTestAsync(string uriString, double x, double y, double width, double height)
        {
            var image = new BitmapImage(new Uri(@"pack://siteoforigin:,,,/" + uriString, UriKind.Absolute)).ToBitmap().ToImage<Rgba, byte>();
            var axis = imageService.GetAxisLocation(image);
            image.Draw(axis.ToRectangle(), new Rgba(0, 0, 255, 255));
            CvInvoke.Imshow("", image);
            CvInvoke.WaitKey();
            Assert.AreEqual(new RectangleD(x, y, width, height), axis);
        }

        [DataTestMethod()]
        [DataRow("Assets/data.png", 77, 17, 759, 560)]
        [DataRow("Assets/Graph-1.jpg", 38, 43, 580, 335)]
        [DataRow("Assets/Hysteresis-loop-of-cobalt-ferrite-samples-CF-CF300_30-and-CF600_180.png", 79, 0, 770, 566)]
        [DataRow("Assets/Inseam-v-Height-Graph.jpg", 48, 7, 644, 422)]
        [DataRow("Assets/rnaseqdedemo_19.png", 63, 31, 377, 343)]
        [DataRow("Assets/scatter_and_hist_border.png", 59, 222, 470, 470)]
        [DataRow("Assets/Screenshot 2021-06-26 230901.png", 104, 16, 838, 648)]
        public void CropImageTest(string uriString, int x, int y, int width, int height)
        {
            var image = new BitmapImage(new Uri(@"pack://siteoforigin:,,,/" + uriString, UriKind.Absolute)).ToBitmap().ToImage<Rgba, byte>();
            var roi = new Rectangle(x, y, width, height);
            var croppedImage = imageService.CropImage(image, roi);
            Assert.AreEqual(roi.Size, croppedImage.Size);
            Assert.AreEqual(image[roi.Y, roi.X], croppedImage[0, 0]);
            Assert.AreEqual(image[roi.Bottom - 1, roi.Right - 1], croppedImage[roi.Height - 1, roi.Width - 1]);
        }

        [DataTestMethod()]
        [DataRow("Assets/data.png")]
        public void CropOutOfRangeTest(string uriString)
        {
            var image = new BitmapImage(new Uri(@"pack://siteoforigin:,,,/" + uriString, UriKind.Absolute)).ToBitmap().ToImage<Rgba, byte>();
            var roi = new Rectangle(int.MinValue, int.MinValue, int.MaxValue, int.MaxValue);
            var croppedImage = imageService.CropImage(image, roi);
            Assert.AreEqual(image.Size, croppedImage.Size);
        }

        [DataTestMethod()]
        [DataRow("Assets/data.png")]
        public void CropZeroRoiTest(string uriString)
        {
            var image = new BitmapImage(new Uri(@"pack://siteoforigin:,,,/" + uriString, UriKind.Absolute)).ToBitmap().ToImage<Rgba, byte>();
            var roi = new Rectangle(0, 0, 0, 0);
            Assert.ThrowsException<ArgumentException>(() =>
            {
                var croppedImage = imageService.CropImage(image, roi);
            });
        }

        [DataTestMethod()]
        [DataRow("Assets/static-test-image.png", 0, 0, 0, 255, 255, 255, 255)]
        [DataRow("Assets/static-test-image.png", 255, 0, 0, 255, 255, 255, 0)]
        public void FilterRGBTest(string uriString, int minR, int minG, int minB, int maxR, int maxG, int maxB, int result)
        {
            var image = new BitmapImage(new Uri(@"pack://siteoforigin:,,,/" + uriString, UriKind.Absolute)).ToBitmap().ToImage<Rgba, byte>();
            var min = new Rgba(minR, minG, minB, byte.MaxValue);
            var max = new Rgba(maxR, maxG, maxB, byte.MaxValue);
            var filteredImage = imageService.FilterRGB(image, min, max);
            Assert.AreEqual(result, filteredImage[0, 0].Alpha);
        }

        [TestMethod()]
        [DataRow("Assets/data.png")]
        public void EraseImageTest(string uriString)
        {
            var image = new BitmapImage(new Uri(@"pack://siteoforigin:,,,/" + uriString, UriKind.Absolute)).ToBitmap().ToImage<Rgba, byte>();
            var roi = new Rectangle(-5, -5, 10, 10);
            image.EraseImage(roi);
            Assert.AreEqual(new Rgba(), image[0, 0]);
        }
    }
}