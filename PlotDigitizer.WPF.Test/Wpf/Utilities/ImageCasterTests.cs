using Emgu.CV;
using Emgu.CV.Structure;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using PlotDigitizer.WPF;

using System.Drawing;

namespace PlotDigitizer.WPF.Tests.Utilities
{
	/// <summary>
	/// The bridge between the Emgu CV images the pipeline works on and the WPF image sources the
	/// views bind to. Transparency has to survive the trip in both directions.
	/// </summary>
	/// <remarks>
	/// A <c>BitmapSource</c> belongs to the thread that created it, so every assertion about one
	/// stays inside the STA delegate.
	/// </remarks>
	[TestClass]
	public class ImageCasterTests
	{
		[TestMethod]
		[TestCategory("Unit")]
		public void ToBitmapSource_Image_KeepsSizeAndColour()
		{
			using var image = new Image<Rgba, byte>(6, 4);
			image[1, 2] = new Rgba(10, 20, 30, 255);

			StaTestContext.Run(() =>
			{
				var source = image.ToBitmapSource();
				Assert.AreEqual(6, source.PixelWidth, "width");
				Assert.AreEqual(4, source.PixelHeight, "height");

				var stride = source.PixelWidth * 4;
				var pixels = new byte[stride * source.PixelHeight];
				source.CopyPixels(pixels, stride, 0);

				var offset = 1 * stride + 2 * 4; // bgra
				Assert.AreEqual(30, pixels[offset + 0], "blue");
				Assert.AreEqual(20, pixels[offset + 1], "green");
				Assert.AreEqual(10, pixels[offset + 2], "red");
				Assert.AreEqual(255, pixels[offset + 3], "alpha");
			});
		}

		[TestMethod]
		[TestCategory("Unit")]
		public void ToBitmapSource_TransparentPixel_KeepsAlpha()
		{
			using var image = new Image<Rgba, byte>(2, 2);
			image[0, 0] = new Rgba(0, 0, 0, 0);
			image[0, 1] = new Rgba(0, 0, 0, 255);

			StaTestContext.Run(() =>
			{
				var source = image.ToBitmapSource();
				var stride = source.PixelWidth * 4;
				var pixels = new byte[stride * source.PixelHeight];
				source.CopyPixels(pixels, stride, 0);

				Assert.AreEqual(0, pixels[3], "alpha of the transparent pixel");
				Assert.AreEqual(255, pixels[7], "alpha of the opaque pixel");
			});
		}

		[TestMethod]
		[TestCategory("Unit")]
		public void ToBitmap_BitmapSource_RoundTripsThroughWpfAndBack()
		{
			using var image = new Image<Rgba, byte>(5, 3);
			image[2, 4] = new Rgba(200, 100, 50, 255);

			using var roundTripped = StaTestContext.Run(() => image.ToBitmapSource().ToBitmap().ToImage<Rgba, byte>());

			Assert.AreEqual(5, roundTripped.Width, "width");
			Assert.AreEqual(3, roundTripped.Height, "height");
			Assert.AreEqual(new Rgba(200, 100, 50, 255), roundTripped[2, 4]);
		}

		[TestMethod]
		[TestCategory("Unit")]
		public void ToBitmap_TransparentBitmapSource_KeepsTransparency()
		{
			using var image = new Image<Rgba, byte>(2, 2);
			image[0, 0] = new Rgba(0, 0, 0, 0);

			using var roundTripped = StaTestContext.Run(() => image.ToBitmapSource().ToBitmap().ToImage<Rgba, byte>());

			Assert.AreEqual(0, roundTripped[0, 0].Alpha, "alpha survives the png encoder");
		}

		[TestMethod]
		[TestCategory("Unit")]
		public void ToBitmapSource_Bitmap_KeepsSize()
		{
			using var bitmap = new Bitmap(8, 6);

			StaTestContext.Run(() =>
			{
				var source = bitmap.ToBitmapSource();
				Assert.AreEqual(8, source.PixelWidth, "width");
				Assert.AreEqual(6, source.PixelHeight, "height");
			});
		}

		[TestMethod]
		[TestCategory("Unit")]
		public void ToBitmapSource_NonRgbaImage_IsConvertedInsteadOfThrowing()
		{
			using var image = new Image<Gray, byte>(3, 3);

			StaTestContext.Run(() =>
			{
				var source = image.ToBitmapSource();
				Assert.AreEqual(3, source.PixelWidth, "width");
				Assert.AreEqual(3, source.PixelHeight, "height");
			});
		}
	}
}
