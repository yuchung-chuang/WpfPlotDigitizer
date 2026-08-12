using Emgu.CV;
using Emgu.CV.Structure;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using PlotDigitizer.WPF;

using System;
using System.Globalization;
using System.Windows.Media.Imaging;

namespace PlotDigitizer.Core.Tests.Wpf.Utilities.ValueConverters
{
	[TestClass]
	public class ImageSourceConverterTests
	{
		private readonly ImageSourceConverter converter = new();

		[TestMethod]
		[TestCategory("Unit")]
		public void Convert_Image_ReturnsBitmapSourceOfTheSameSize()
		{
			using var image = new Image<Rgba, byte>(7, 5);

			StaTestContext.Run(() =>
			{
				var result = converter.Convert(image, typeof(BitmapSource), null, CultureInfo.InvariantCulture) as BitmapSource;

				Assert.IsNotNull(result);
				Assert.AreEqual(7, result.PixelWidth, "width");
				Assert.AreEqual(5, result.PixelHeight, "height");
			});
		}

		[DataTestMethod]
		[DataRow(null)]
		[DataRow("not an image")]
		[TestCategory("Unit")]
		public void Convert_NotAnRgbaImage_ReturnsNull(object value)
		{
			Assert.IsNull(converter.Convert(value, typeof(BitmapSource), null, CultureInfo.InvariantCulture));
		}

		[TestMethod]
		[TestCategory("Unit")]
		public void Convert_ImageOfAnotherDepth_ReturnsNull()
		{
			using var image = new Image<Bgr, byte>(4, 4);

			Assert.IsNull(converter.Convert(image, typeof(BitmapSource), null, CultureInfo.InvariantCulture));
		}

		[TestMethod]
		[TestCategory("Unit")]
		public void ConvertBack_Always_Throws()
		{
			Assert.ThrowsException<NotImplementedException>(
				() => converter.ConvertBack(null, typeof(Image<Rgba, byte>), null, CultureInfo.InvariantCulture));
		}
	}
}
