using Microsoft.VisualStudio.TestTools.UnitTesting;


using System;
using System.Globalization;
using System.Windows;

namespace PlotDigitizer.WPF.Tests.Utilities.ValueConverters
{
	[TestClass]
	public class ExtendMarginConverterTests
	{
		private readonly ExtendMarginConverter converter = new();

		private object Convert(params object[] values)
			=> converter.Convert(values, typeof(Thickness), null, CultureInfo.InvariantCulture);

		[TestMethod]
		[TestCategory("Unit")]
		public void Convert_MarginAndThickness_ExpandsLeftAndTopOnly()
		{
			var result = Convert(new Thickness(10, 20, 0, 0), 2d);

			Assert.AreEqual(new Thickness(8, 18, 0, 0), result);
		}

		[TestMethod]
		[TestCategory("Unit")]
		public void Convert_ZeroThickness_ReturnsSameLeftAndTop()
		{
			var result = Convert(new Thickness(10, 20, 5, 5), 0d);

			Assert.AreEqual(new Thickness(10, 20, 0, 0), result);
		}

		[TestMethod]
		[TestCategory("Unit")]
		public void Convert_WrongNumberOfValues_ReturnsNull()
		{
			Assert.IsNull(Convert(new Thickness(1)), "one value");
			Assert.IsNull(Convert(new Thickness(1), 2d, 3d), "three values");
			Assert.IsNull(Convert(), "no values");
		}

		[TestMethod]
		[TestCategory("Unit")]
		public void Convert_WrongValueTypes_ReturnsNull()
		{
			Assert.IsNull(Convert("10,20,0,0", 2d), "margin is not a Thickness");
			Assert.IsNull(Convert(new Thickness(1), 2), "thickness is not a double");
			Assert.IsNull(Convert(null, null), "both null");
		}

		[TestMethod]
		[TestCategory("Unit")]
		public void ConvertBack_Always_Throws()
		{
			Assert.ThrowsException<NotImplementedException>(
				() => converter.ConvertBack(new Thickness(1), new[] { typeof(Thickness), typeof(double) }, null, CultureInfo.InvariantCulture));
		}
	}
}
