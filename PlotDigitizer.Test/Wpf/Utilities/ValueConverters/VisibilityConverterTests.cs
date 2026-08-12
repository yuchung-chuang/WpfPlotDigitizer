using Microsoft.VisualStudio.TestTools.UnitTesting;

using PlotDigitizer.WPF;

using System;
using System.Globalization;
using System.Windows;

namespace PlotDigitizer.Core.Tests.Wpf.Utilities.ValueConverters
{
	[TestClass]
	public class VisibilityConverterTests
	{
		private readonly VisibilityConverter converter = new();

		[DataTestMethod]
		[DataRow(true, Visibility.Visible)]
		[DataRow(false, Visibility.Collapsed)]
		[TestCategory("Unit")]
		public void Convert_Boolean_ReturnsMatchingVisibility(bool value, Visibility expected)
		{
			Assert.AreEqual(expected, converter.Convert(value, typeof(Visibility), null, CultureInfo.InvariantCulture));
		}

		[DataTestMethod]
		[DataRow(null)]
		[DataRow("true")]
		[DataRow(1)]
		[TestCategory("Unit")]
		public void Convert_NotABoolean_ReturnsNull(object value)
		{
			Assert.IsNull(converter.Convert(value, typeof(Visibility), null, CultureInfo.InvariantCulture));
		}

		[TestMethod]
		[TestCategory("Unit")]
		public void ConvertBack_Always_Throws()
		{
			Assert.ThrowsException<NotImplementedException>(
				() => converter.ConvertBack(Visibility.Visible, typeof(bool), null, CultureInfo.InvariantCulture));
		}
	}
}
