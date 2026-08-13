using FlaUI.Core.AutomationElements;
using FlaUI.Core.Definitions;
using FlaUI.Core.Tools;
using FlaUI.UIA3;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Linq;

using Application = FlaUI.Core.Application;

namespace PlotDigitizer.WPF.Tests.UI
{
	/// <summary>
	/// Drives the real desktop client through UI automation. Elements are addressed by the
	/// automation ids that the WPF project documents as a test contract, so these tests fail when
	/// an id is renamed, and pages are identified by the header that shows the current page name.
	/// </summary>
	/// <remarks>
	/// Clicking is a physical mouse click, so it is only delivered once the window is in the
	/// foreground; <see cref="NavigateTo"/> therefore re-tries until the page actually changes.
	/// </remarks>
	[TestClass]
	[TestCategory("UI")]
	public class MainWindowUiTests
	{
		private static readonly TimeSpan timeout = TimeSpan.FromSeconds(10);

		private Application app;
		private UIA3Automation automation;
		private Window mainWindow;

		[TestInitialize]
		public void OnTestInitialize()
		{
			app = Application.Launch("PlotDigitizer.exe");
			app.WaitWhileMainHandleIsMissing();
			automation = new UIA3Automation();
			// the splash screen is the process main window at startup, so the real window is
			// located by its automation id instead
			mainWindow = Retry.WhileNull(
				() => app.GetAllTopLevelWindows(automation).FirstOrDefault(w => w.AutomationId == "PlotDigitizer"),
				timeout,
				ignoreException: true).Result;
			Assert.IsNotNull(mainWindow, "the main window did not appear");
			Assert.IsNotNull(Find("LoadPageItem"), "the main window did not finish loading");
		}

		[TestCleanup]
		public void OnTestCleanup()
		{
			automation?.Dispose();
			// a leftover window would swallow the clicks of the next test
			app?.Close();
			if (app?.HasExited == false) {
				app.Kill();
			}
			app?.Dispose();
		}

		private AutomationElement Find(string automationId, TimeSpan? searchTimeout = null)
			=> Retry.WhileNull(
				() => mainWindow.FindFirstDescendant(cf => cf.ByAutomationId(automationId)),
				searchTimeout ?? timeout,
				ignoreException: true).Result;

		private void AssertPresent(params string[] automationIds)
		{
			foreach (var id in automationIds) {
				Assert.IsNotNull(Find(id), $"'{id}' should be reachable by automation id");
			}
		}

		private string CurrentPageName()
			=> mainWindow
				.FindAllDescendants(cf => cf.ByControlType(ControlType.Text))
				.Select(text => text.Name)
				.FirstOrDefault(name => name != null && name.EndsWith("Page"));

		private void AssertCurrentPage(string expected)
		{
			var actual = Retry.WhileNull(
				() => CurrentPageName() == expected ? expected : null,
				timeout,
				ignoreException: true).Result;
			Assert.AreEqual(expected, actual, "the header should name the current page");
		}

		private void NavigateTo(string pageItemId, string expectedPageName)
		{
			for (var attempt = 0; attempt < 3; attempt++) {
				mainWindow.SetForeground();
				var item = Find(pageItemId);
				Assert.IsNotNull(item, $"'{pageItemId}' should be reachable by automation id");
				item.Click();
				var arrived = Retry.WhileNull(
					() => CurrentPageName() == expectedPageName ? expectedPageName : null,
					TimeSpan.FromSeconds(3),
					ignoreException: true).Result;
				if (arrived != null) {
					return;
				}
			}
			Assert.Fail($"clicking '{pageItemId}' did not open '{expectedPageName}', the header still reads '{CurrentPageName()}'");
		}

		[TestMethod]
		public void MainWindow_OnLaunch_ShowsEveryNavigationItem()
		{
			AssertPresent("LoadPageItem", "AxisPageItem", "RangePageItem", "FilterPageItem", "EditPageItem", "DataPageItem");
		}

		[TestMethod]
		public void MainWindow_OnLaunch_StartsOnTheLoadPage()
		{
			AssertCurrentPage("Load Page");
			AssertPresent("BrowseButton", "PasteButton", "filePath");
		}

		[TestMethod]
		public void Navigation_AxisPage_ShowsTheAxisSelection()
		{
			NavigateTo("AxisPageItem", "Axis Page");

			AssertPresent("ImageControl");
		}

		[TestMethod]
		public void Navigation_RangePage_ShowsEveryAxisLimitInput()
		{
			NavigateTo("RangePageItem", "Axis Range Page");

			AssertPresent("XMin", "XMax", "XLog", "YMin", "YMax", "YLog", "XLabel", "YLabel");
		}

		[TestMethod]
		public void Navigation_FilterPage_ShowsTheThreeColourSliders()
		{
			NavigateTo("FilterPageItem", "Filter Page");

			var thumbs = mainWindow.FindAllDescendants(cf => cf.ByAutomationId("PART_MiddleThumb"));

			Assert.AreEqual(3, thumbs.Length, "one range slider per colour channel");
		}

		[TestMethod]
		public void Navigation_EditPage_ShowsTheEditorToolbar()
		{
			NavigateTo("EditPageItem", "Edit Page");

			AssertPresent("PencilButton", "EraserButton", "RectButton", "PolyButton", "UndoButton");
		}

		[TestMethod]
		public void Navigation_DataPage_ShowsTheExportControls()
		{
			NavigateTo("DataPageItem", "Data Page");

			AssertPresent("exportButton", "continuousButton", "discreteButton");
		}

		[TestMethod]
		public void Navigation_BackToTheLoadPage_ShowsTheLoadControlsAgain()
		{
			NavigateTo("DataPageItem", "Data Page");

			NavigateTo("LoadPageItem", "Load Page");

			AssertPresent("BrowseButton", "PasteButton");
		}
	}
}
