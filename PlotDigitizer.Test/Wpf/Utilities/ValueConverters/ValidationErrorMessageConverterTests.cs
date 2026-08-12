using Microsoft.VisualStudio.TestTools.UnitTesting;

using PlotDigitizer.WPF;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Controls;

namespace PlotDigitizer.Core.Tests.Wpf.Utilities.ValueConverters
{
	[TestClass]
	public class ValidationErrorMessageConverterTests
	{
		private readonly ValidationErrorMessageConverter converter = new();

		private static ReadOnlyCollection<ValidationError> Errors(params object[] contents)
		{
			var errors = new List<ValidationError>();
			foreach (var content in contents) {
				errors.Add(new ValidationError(new NumberValidation(), new object()) { ErrorContent = content });
			}
			return new ReadOnlyCollection<ValidationError>(errors);
		}

		private object Convert(object value)
			=> converter.Convert(value, typeof(string), null, CultureInfo.InvariantCulture);

		[TestMethod]
		[TestCategory("Unit")]
		public void Convert_SingleError_ReturnsItsMessageOnOneLine()
		{
			Assert.AreEqual("boom" + Environment.NewLine, Convert(Errors("boom")));
		}

		[TestMethod]
		[TestCategory("Unit")]
		public void Convert_MultipleErrors_ConcatenatesEveryMessage()
		{
			Assert.AreEqual(
				"first" + Environment.NewLine + "second" + Environment.NewLine,
				Convert(Errors("first", "second")));
		}

		[TestMethod]
		[TestCategory("Unit")]
		public void Convert_ErrorWithoutContent_IsSkipped()
		{
			Assert.AreEqual("kept" + Environment.NewLine, Convert(Errors(null, "kept")));
		}

		[TestMethod]
		[TestCategory("Unit")]
		public void Convert_NoErrors_ReturnsEmptyString()
		{
			Assert.AreEqual(string.Empty, Convert(Errors()));
		}

		[DataTestMethod]
		[DataRow(null)]
		[DataRow("already a message")]
		[TestCategory("Unit")]
		public void Convert_NotAnErrorCollection_ReturnsNull(object value)
		{
			Assert.IsNull(Convert(value));
		}

		[TestMethod]
		[TestCategory("Unit")]
		public void ConvertBack_Always_Throws()
		{
			Assert.ThrowsException<NotImplementedException>(
				() => converter.ConvertBack("message", typeof(ReadOnlyCollection<ValidationError>), null, CultureInfo.InvariantCulture));
		}
	}
}
