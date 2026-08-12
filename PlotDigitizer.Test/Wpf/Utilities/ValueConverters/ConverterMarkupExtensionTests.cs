using Microsoft.VisualStudio.TestTools.UnitTesting;

using PlotDigitizer.WPF;

namespace PlotDigitizer.Core.Tests.Wpf.Utilities.ValueConverters
{
	/// <summary>
	/// Both converter base classes are markup extensions that hand XAML a single shared instance
	/// instead of the one the parser created, so <c>{local:RectConverter}</c> never allocates per
	/// binding.
	/// </summary>
	[TestClass]
	public class ConverterMarkupExtensionTests
	{
		[TestMethod]
		[TestCategory("Unit")]
		public void ProvideValue_ValueConverter_AlwaysReturnsTheSameSharedInstance()
		{
			var first = new RectConverter().ProvideValue(null);
			var second = new RectConverter().ProvideValue(null);

			Assert.IsInstanceOfType(first, typeof(RectConverter));
			Assert.AreSame(first, second);
		}

		[TestMethod]
		[TestCategory("Unit")]
		public void ProvideValue_ValueConverter_IsNotTheParsedInstance()
		{
			var parsed = new DoubleToStringConverter();

			Assert.AreNotSame(parsed, parsed.ProvideValue(null));
		}

		[TestMethod]
		[TestCategory("Unit")]
		public void ProvideValue_MultiValueConverter_AlwaysReturnsTheSameSharedInstance()
		{
			var first = new ExtendMarginConverter().ProvideValue(null);
			var second = new ExtendMarginConverter().ProvideValue(null);

			Assert.IsInstanceOfType(first, typeof(ExtendMarginConverter));
			Assert.AreSame(first, second);
		}

		[TestMethod]
		[TestCategory("Unit")]
		public void ProvideValue_DifferentConverters_ReturnDifferentInstances()
		{
			Assert.AreNotSame(
				new EditorStateConverter().ProvideValue(null),
				new ExtendMarginConverter().ProvideValue(null));
		}
	}
}
