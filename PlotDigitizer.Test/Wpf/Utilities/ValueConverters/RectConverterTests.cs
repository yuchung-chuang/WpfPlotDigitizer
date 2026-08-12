using Microsoft.VisualStudio.TestTools.UnitTesting;

using PlotDigitizer.WPF;

using System;
using System.Globalization;
using System.Windows;

using RectConverter = PlotDigitizer.WPF.RectConverter;

namespace PlotDigitizer.Core.Tests.Wpf.Utilities.ValueConverters
{
	[TestClass]
	public class RectConverterTests
	{
		private readonly RectConverter converter = new();

		[TestMethod]
		[TestCategory("Unit")]
		public void Convert_RectangleD_ReturnsEquivalentRect()
		{
			var result = converter.Convert(new RectangleD(1.5, 2.5, 30, 40), typeof(Rect), null, CultureInfo.InvariantCulture);

			Assert.AreEqual(new Rect(1.5, 2.5, 30, 40), result);
		}

		[DataTestMethod]
		[DataRow(null)]
		[DataRow("not a rectangle")]
		[TestCategory("Unit")]
		public void Convert_NotARectangleD_ReturnsNull(object value)
		{
			Assert.IsNull(converter.Convert(value, typeof(Rect), null, CultureInfo.InvariantCulture));
		}

		/// <summary>
		/// <see cref="RectangleD"/> tolerates a negative size but <see cref="Rect"/> does not, so
		/// an inverted selection cannot be converted for display.
		/// </summary>
		[TestMethod]
		[TestCategory("Unit")]
		public void Convert_NegativeSize_Throws()
		{
			Assert.ThrowsException<ArgumentException>(
				() => converter.Convert(new RectangleD(0, 0, -10, 10), typeof(Rect), null, CultureInfo.InvariantCulture));
		}

		[TestMethod]
		[TestCategory("Unit")]
		public void ConvertBack_Rect_ReturnsEquivalentRectangleD()
		{
			var result = converter.ConvertBack(new Rect(1.5, 2.5, 30, 40), typeof(RectangleD), null, CultureInfo.InvariantCulture);

			Assert.AreEqual(new RectangleD(1.5, 2.5, 30, 40), result);
		}

		[DataTestMethod]
		[DataRow(null)]
		[DataRow("not a rect")]
		[TestCategory("Unit")]
		public void ConvertBack_NotARect_ReturnsNull(object value)
		{
			Assert.IsNull(converter.ConvertBack(value, typeof(RectangleD), null, CultureInfo.InvariantCulture));
		}

		[TestMethod]
		[TestCategory("Unit")]
		public void Convert_RoundTrip_PreservesValue()
		{
			var original = new RectangleD(3, 4, 5, 6);

			var rect = converter.Convert(original, typeof(Rect), null, CultureInfo.InvariantCulture);

			Assert.AreEqual(original, converter.ConvertBack(rect, typeof(RectangleD), null, CultureInfo.InvariantCulture));
		}

		[TestMethod]
		[TestCategory("Unit")]
		public void Convert_EmptyRectangleD_ReturnsEmptySizedRect()
		{
			var result = (Rect)converter.Convert(default(RectangleD), typeof(Rect), null, CultureInfo.InvariantCulture);

			Assert.AreEqual(new Rect(0, 0, 0, 0), result);
		}
	}
}
