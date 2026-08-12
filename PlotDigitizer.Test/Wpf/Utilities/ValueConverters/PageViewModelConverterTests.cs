using Microsoft.VisualStudio.TestTools.UnitTesting;

using PlotDigitizer.WPF;

using System;
using System.Globalization;
using System.Windows.Controls;

namespace PlotDigitizer.Core.Tests.Wpf.Utilities.ValueConverters
{
	/// <summary>
	/// The navigation binds the selected <see cref="TabItem"/> back to the page view model, so
	/// only the "tab hosting a page whose data context is a view model" shape may convert.
	/// </summary>
	[TestClass]
	public class PageViewModelConverterTests
	{
		private readonly PageViewModelConverter converter = new();

		private object ConvertBack(object value)
			=> converter.ConvertBack(value, typeof(ViewModelBase), null, CultureInfo.InvariantCulture);

		[TestMethod]
		[TestCategory("Unit")]
		public void ConvertBack_TabItemHostingAPage_ReturnsItsViewModel()
		{
			var viewModel = new StubPage();

			var result = StaTestContext.Run(() =>
			{
				var page = new UserControl { DataContext = viewModel };
				return ConvertBack(new TabItem { Content = page });
			});

			Assert.AreSame(viewModel, result);
		}

		[TestMethod]
		[TestCategory("Unit")]
		public void ConvertBack_TabItemHostingSomethingElse_ReturnsNull()
		{
			var result = StaTestContext.Run(() => ConvertBack(new TabItem { Content = new Button() }));

			Assert.IsNull(result);
		}

		[TestMethod]
		[TestCategory("Unit")]
		public void ConvertBack_PageWithoutAViewModel_ReturnsNull()
		{
			var result = StaTestContext.Run(
				() => ConvertBack(new TabItem { Content = new UserControl { DataContext = "not a view model" } }));

			Assert.IsNull(result);
		}

		[DataTestMethod]
		[DataRow(null)]
		[DataRow("not a tab item")]
		[TestCategory("Unit")]
		public void ConvertBack_NotATabItem_ReturnsNull(object value)
		{
			Assert.IsNull(ConvertBack(value));
		}

		[TestMethod]
		[TestCategory("Unit")]
		public void Convert_Always_Throws()
		{
			Assert.ThrowsException<NotImplementedException>(
				() => converter.Convert(new StubPage(), typeof(TabItem), null, CultureInfo.InvariantCulture));
		}
	}
}
