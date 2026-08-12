using Microsoft.VisualStudio.TestTools.UnitTesting;

using PlotDigitizer.WPF;

using System;
using System.Globalization;

namespace PlotDigitizer.Core.Tests.Wpf.Utilities.ValueConverters
{
	[TestClass]
	public class DoubleToStringConverterTests
	{
		private readonly DoubleToStringConverter converter = new();

		[TestMethod]
		[TestCategory("Unit")]
		public void Convert_Double_ReturnsItsStringForm()
		{
			Assert.AreEqual("12", converter.Convert(12d, typeof(string), null, CultureInfo.InvariantCulture));
		}

		[TestMethod]
		[TestCategory("Unit")]
		public void Convert_NaN_ReturnsEmptyString()
		{
			Assert.AreEqual(string.Empty, converter.Convert(double.NaN, typeof(string), null, CultureInfo.InvariantCulture));
		}

		[DataTestMethod]
		[DataRow(null)]
		[DataRow("12")]
		[DataRow(12)]
		[TestCategory("Unit")]
		public void Convert_NotADouble_ReturnsNull(object value)
		{
			Assert.IsNull(converter.Convert(value, typeof(string), null, CultureInfo.InvariantCulture));
		}

		[DataTestMethod]
		[DataRow("")]
		[DataRow("   ")]
		[TestCategory("Unit")]
		public void ConvertBack_BlankText_ReturnsNaN(string value)
		{
			Assert.AreEqual(double.NaN, converter.ConvertBack(value, typeof(double), null, CultureInfo.InvariantCulture));
		}

		[TestMethod]
		[TestCategory("Unit")]
		public void ConvertBack_ParsableText_ReturnsDouble()
		{
			Assert.AreEqual(-12d, converter.ConvertBack("-12", typeof(double), null, CultureInfo.InvariantCulture));
		}

		[TestMethod]
		[TestCategory("Unit")]
		public void ConvertBack_UnparsableText_ReturnsNull()
		{
			Assert.IsNull(converter.ConvertBack("abc", typeof(double), null, CultureInfo.InvariantCulture));
		}

		/// <summary>
		/// The converter calls <c>value.ToString()</c> before any null check, so a null binding
		/// value throws rather than round-tripping to <see cref="double.NaN"/>.
		/// </summary>
		[TestMethod]
		[TestCategory("Unit")]
		public void ConvertBack_Null_Throws()
		{
			Assert.ThrowsException<NullReferenceException>(
				() => converter.ConvertBack(null, typeof(double), null, CultureInfo.InvariantCulture));
		}

		[TestMethod]
		[TestCategory("Unit")]
		public void Convert_RoundTrip_PreservesValue()
		{
			var text = converter.Convert(42d, typeof(string), null, CultureInfo.CurrentCulture);
			Assert.AreEqual(42d, converter.ConvertBack(text, typeof(double), null, CultureInfo.CurrentCulture));
		}
	}
}
