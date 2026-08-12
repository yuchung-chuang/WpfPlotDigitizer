using Microsoft.VisualStudio.TestTools.UnitTesting;

using PlotDigitizer.WPF;

using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PlotDigitizer.Core.Tests.Wpf.Services
{
	/// <summary>
	/// Runs a long digitization step off the UI thread while showing a cancellable progress
	/// popup and a wait cursor.
	/// </summary>
	[TestClass]
	public class AwaitTaskServiceTests
	{
		private static readonly TimeSpan timeout = TimeSpan.FromSeconds(10);

		private static void Wait(Task task)
			=> Assert.IsTrue(task.Wait(timeout), "the task did not complete in time");

		[TestMethod]
		[TestCategory("Unit")]
		public void RunAsync_Function_ReturnsItsResult()
		{
			var service = new AwaitTaskService();

			var task = StaTestContext.Run(() => service.RunAsync(token => 42));
			Wait(task);

			Assert.AreEqual(42, task.Result);
		}

		[TestMethod]
		[TestCategory("Unit")]
		public void RunAsync_Action_RunsItOffTheUiThread()
		{
			var service = new AwaitTaskService();
			var uiThread = StaTestContext.Run(() => Thread.CurrentThread.ManagedThreadId);
			var workerThread = 0;

			var task = StaTestContext.Run(() => service.RunAsync(token => workerThread = Thread.CurrentThread.ManagedThreadId));
			Wait(task);

			Assert.AreNotEqual(uiThread, workerThread, "the work must not block the UI thread");
		}

		[TestMethod]
		[TestCategory("Unit")]
		public void RunAsync_AsyncFunction_ReturnsItsResult()
		{
			var service = new AwaitTaskService();

			var task = StaTestContext.Run(() => service.RunAsync(token => Task.FromResult("done")));
			Wait(task);

			Assert.AreEqual("done", task.Result);
		}

		[TestMethod]
		[TestCategory("Unit")]
		public void RunAsync_Always_ShowsTheWaitCursorAndClearsItAfterwards()
		{
			var service = new AwaitTaskService();
			using var release = new ManualResetEventSlim();

			var task = StaTestContext.Run(() => service.RunAsync(token =>
			{
				release.Wait(timeout);
				return 0;
			}));
			var cursorDuringRun = StaTestContext.Run(() => Mouse.OverrideCursor);
			release.Set();
			Wait(task);
			var cursorAfterRun = StaTestContext.Run(() => Mouse.OverrideCursor);

			Assert.AreSame(Cursors.Wait, cursorDuringRun, "cursor while running");
			Assert.IsNull(cursorAfterRun, "cursor after running");
		}

		[TestMethod]
		[TestCategory("Unit")]
		public void RunAsync_Always_HandsTheWorkACancellableToken()
		{
			var service = new AwaitTaskService();
			var canBeCanceled = false;

			var task = StaTestContext.Run(() => service.RunAsync(token => canBeCanceled = token.CanBeCanceled));
			Wait(task);

			Assert.IsTrue(canBeCanceled, "the popup's cancel button needs a cancellable token");
		}

		[TestMethod]
		[TestCategory("Unit")]
		public void RunAsync_WorkThatThrows_SurfacesTheException()
		{
			var service = new AwaitTaskService();

			var task = StaTestContext.Run(() => service.RunAsync(token => throw new InvalidOperationException("boom")));

			var exception = Assert.ThrowsException<AggregateException>(() => task.Wait(timeout));
			Assert.IsInstanceOfType(exception.InnerException, typeof(InvalidOperationException));
		}
	}
}
