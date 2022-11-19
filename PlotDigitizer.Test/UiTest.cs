
using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using FlaUI.Core;
using FlaUI.UIA3;
using System.Collections.Generic;
using System.Threading;
using FlaUI.Core.Input;
using FlaUI.Core.WindowsAPI;
using System.Diagnostics;
using FlaUI.Core.AutomationElements;

namespace PlotDigitizer.Core.Tests
{
	[TestClass()]
	public class UiTest
	{
		private Application app;
		private UIA3Automation automation;

		[TestInitialize]
		public void OnTestInitialize()
		{
			if (!UriParser.IsKnownScheme("pack"))
				new System.Windows.Application();

			app = Application.Launch(@"C:\Users\alex\source\repos\PlotDigitizer\PlotDigitizer.App\bin\Debug\netcoreapp3.1\PlotDigitizer.App.exe");
			app.WaitWhileMainHandleIsMissing();
			automation = new UIA3Automation();
		}

		[TestCleanup]
		public void OnTestCleanup()
		{
			app.Close();
			app.Dispose();
			automation.Dispose();
		}

		[TestMethod]
		public void ApplicationTest()
		{
			var mainWindow = app.GetMainWindow(automation);
			Assert.IsNotNull(mainWindow);
		}

		[TestMethod]
		public void NavigationTest()
		{
			var mainWindow = app.GetMainWindow(automation);
			var items = new List<FlaUI.Core.AutomationElements.AutomationElement>
			{
				mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("LoadPageItem")),
				mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("RangePageItem")),
				mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("FilterPageItem")),
				mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("EditPageItem")),
				mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("DataPageItem")),
			};
			var random = new Random();
			for (var i = 0; i < 20; i++) {
				var idx = random.Next(items.Count);
				var item = items[idx];
				item?.Click();
				Thread.Sleep(100);
			}
		}

		[TestMethod]
		public void RandomNavigationByKeyTest()
		{
			var random = new Random();
			for (var i = 0; i < 20; i++) {
				var idx = random.Next(2);
				if (idx == 0) {
					Keyboard.Type(VirtualKeyShort.UP);
				} else if (idx == 1) {
					Keyboard.Type(VirtualKeyShort.DOWN);
				}
				Thread.Sleep(100);
			}
		}

		[TestMethod]
		public void NavigationByKeyTest()
		{
			for (var i = 0; i < 10; i++) {
				Keyboard.Type(VirtualKeyShort.DOWN);
				Thread.Sleep(100);
			}
			for (var i = 0; i < 10; i++) {
				Keyboard.Type(VirtualKeyShort.UP);
				Thread.Sleep(100);
			}
		}

		[TestMethod]
		public void LoadByClipboardTest()
		{
			while (app.GetAllTopLevelWindows(automation).Length == 0) {
				Thread.Sleep(100);
			}
			var mainWindow = app.GetAllTopLevelWindows(automation)[0];
			var textbox = mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("filePath")).AsTextBox();
			textbox.Text = @"C:\Users\alex\source\repos\PlotDigitizer\PlotDigitizer.Test\Assets\Screenshot 2021-06-26 230901.png";
			Keyboard.Type(VirtualKeyShort.TAB);
			Thread.Sleep(100);

			textbox = mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("AxLimYMax")).AsTextBox();
			textbox.Text = "130";
			textbox = mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("AxLimXMin")).AsTextBox();
			textbox.Text = "700";
			textbox = mainWindow.FindFirstDescendant(cf => cf.ByAutomationId("AxLimXMax")).AsTextBox();
			textbox.Text = "1050";
			Keyboard.Type(VirtualKeyShort.TAB);
			Thread.Sleep(100);
			Keyboard.Type(VirtualKeyShort.DOWN);
			Thread.Sleep(100);

			Keyboard.Type(VirtualKeyShort.DOWN);
			Thread.Sleep(100);
		}
	}
}