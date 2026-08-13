using Microsoft.VisualStudio.TestTools.UnitTesting;

using PlotDigitizer.Core.Tests;

using System;

namespace PlotDigitizer.WPF.Tests.Services
{
	/// <summary>
	/// Owns the windows that host a view model. <c>ShowDialog</c> is not covered because it
	/// blocks on a modal message loop, which a test cannot unblock without a user.
	/// </summary>
	[TestClass]
	public class WindowServiceTests
	{
		[TestMethod]
		[TestCategory("Unit")]
		public void ShowWindow_Always_EntersTheViewModel()
		{
			var viewModel = new StubPage();
			var service = new WindowService();

			StaTestContext.Run(() =>
			{
				service.ShowWindow(viewModel);
				service.CloseWindow(viewModel);
			});

			Assert.AreEqual(1, viewModel.EnterCount);
		}

		[TestMethod]
		[TestCategory("Unit")]
		public void CloseWindow_AnOpenWindow_ClosesItAndLeavesTheViewModel()
		{
			var viewModel = new StubPage();
			var service = new WindowService();

			var closed = StaTestContext.Run(() =>
			{
				service.ShowWindow(viewModel);
				return service.CloseWindow(viewModel);
			});

			Assert.IsTrue(closed, "the window was found");
			Assert.AreEqual(1, viewModel.LeaveCount, "leave count");
		}

		[TestMethod]
		[TestCategory("Unit")]
		public void CloseWindow_AViewModelThatWasNeverShown_ReturnsFalse()
		{
			var service = new WindowService();

			var closed = StaTestContext.Run(() => service.CloseWindow(new StubPage()));

			Assert.IsFalse(closed);
		}

		[TestMethod]
		[TestCategory("Unit")]
		public void CloseDialog_AViewModelThatWasNeverShown_ReturnsFalse()
		{
			var service = new WindowService();

			var closed = StaTestContext.Run(() => service.CloseDialog(new StubPage(), true));

			Assert.IsFalse(closed);
		}

		/// <summary>
		/// A window opened with <c>ShowWindow</c> is not modal, so WPF refuses the dialog result.
		/// </summary>
		[TestMethod]
		[TestCategory("Unit")]
		public void CloseDialog_AWindowThatWasNotShownAsADialog_Throws()
		{
			var viewModel = new StubPage();
			var service = new WindowService();

			StaTestContext.Run(() =>
			{
				service.ShowWindow(viewModel);
				try {
					Assert.ThrowsException<InvalidOperationException>(() => service.CloseDialog(viewModel, true));
				}
				finally {
					service.CloseWindow(viewModel);
				}
			});
		}

		[TestMethod]
		[TestCategory("Unit")]
		public void CloseWindow_Twice_ReportsTheStaleRegistrationButLeavesTheViewModelOnce()
		{
			var viewModel = new StubPage();
			var service = new WindowService();

			var secondClose = StaTestContext.Run(() =>
			{
				service.ShowWindow(viewModel);
				service.CloseWindow(viewModel);
				return service.CloseWindow(viewModel);
			});

			Assert.IsTrue(secondClose, "closed windows stay in the service's list");
			Assert.AreEqual(1, viewModel.LeaveCount, "closing an already closed window does not leave twice");
		}
	}
}
