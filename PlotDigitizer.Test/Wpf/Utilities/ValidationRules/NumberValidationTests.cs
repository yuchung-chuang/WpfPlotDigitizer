using Microsoft.VisualStudio.TestTools.UnitTesting;

using PlotDigitizer.WPF;

using System;
using System.Globalization;

namespace PlotDigitizer.Core.Tests.Wpf.Utilities.ValidationRules
{
	/// <summary>
	/// Guards the axis-limit text boxes: a blank box is allowed (it means "unset"), anything else
	/// has to parse as a double.
	/// </summary>
	[TestClass]
	public class NumberValidationTests
	{
		private readonly NumberValidation rule = new();

		[DataTestMethod]
		[DataRow("12")]
		[DataRow("-12.5")]
		[DataRow("1e3")]
		[DataRow("")]
		[DataRow("   ")]
		[TestCategory("Unit")]
		public void Validate_BlankOrNumericText_IsValid(string value)
		{
			var result = rule.Validate(value, CultureInfo.InvariantCulture);

			Assert.IsTrue(result.IsValid, $"'{value}' should be accepted");
		}

		[DataTestMethod]
		[DataRow("abc")]
		[DataRow("12abc")]
		[DataRow("1.2.3")]
		[TestCategory("Unit")]
		public void Validate_NonNumericText_IsInvalidWithMessage(string value)
		{
			var result = rule.Validate(value, CultureInfo.InvariantCulture);

			Assert.IsFalse(result.IsValid, $"'{value}' should be rejected");
			Assert.AreEqual("Input value is not a double.", result.ErrorContent);
		}

		[TestMethod]
		[TestCategory("Unit")]
		public void Validate_NonStringValue_UsesItsTextForm()
		{
			Assert.IsTrue(rule.Validate(12d, CultureInfo.InvariantCulture).IsValid, "a double is valid");
			Assert.IsFalse(rule.Validate(new object(), CultureInfo.InvariantCulture).IsValid, "an arbitrary object is not");
		}

		/// <summary>
		/// The rule dereferences the value before checking it, so a null binding value throws
		/// instead of failing validation.
		/// </summary>
		[TestMethod]
		[TestCategory("Unit")]
		public void Validate_Null_Throws()
		{
			Assert.ThrowsException<NullReferenceException>(() => rule.Validate(null, CultureInfo.InvariantCulture));
		}
	}
}
