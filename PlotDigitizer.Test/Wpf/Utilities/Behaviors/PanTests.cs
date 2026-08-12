using Microsoft.VisualStudio.TestTools.UnitTesting;

using PlotDigitizer.WPF;

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace PlotDigitizer.Core.Tests.Wpf.Utilities.Behaviors
{
	/// <summary>
	/// The pan behaviour attached to the editor image. Mouse capture needs a live window, so
	/// these tests cover the parts that are observable without one: the transform setup, the
	/// gesture filter and the cursor swap.
	/// </summary>
	[TestClass]
	public class PanTests
	{
		private static (Grid parent, Image element) CreatePannableImage()
		{
			var element = new Image { Width = 100, Height = 100, Cursor = Cursors.Arrow };
			var parent = new Grid();
			parent.Children.Add(element);
			Pan.SetIsEnabled(element, true);
			return (parent, element);
		}

		private static void MouseDown(UIElement element, MouseButton button)
			=> element.RaiseEvent(new MouseButtonEventArgs(Mouse.PrimaryDevice, 0, button)
			{
				RoutedEvent = UIElement.MouseDownEvent,
			});

		[TestMethod]
		[TestCategory("Unit")]
		public void SetIsEnabled_True_PreparesTransformsAndClipsTheParent()
		{
			StaTestContext.Run(() =>
			{
				var (parent, element) = CreatePannableImage();

				Assert.IsInstanceOfType(element.RenderTransform, typeof(TransformGroup));
				Assert.AreEqual(4, ((TransformGroup)element.RenderTransform).Children.Count);
				Assert.IsTrue(parent.ClipToBounds, "the parent must clip the panned content");
			});
		}

		[TestMethod]
		[TestCategory("Unit")]
		public void MouseDown_MatchingGesture_SwitchesToThePanCursor()
		{
			StaTestContext.Run(() =>
			{
				var (_, element) = CreatePannableImage();

				MouseDown(element, MouseButton.Left);

				Assert.AreNotSame(Cursors.Arrow, element.Cursor, "the pan cursor must be applied");
			});
		}

		[TestMethod]
		[TestCategory("Unit")]
		public void MouseDown_GestureDoesNotMatch_LeavesTheCursorAlone()
		{
			StaTestContext.Run(() =>
			{
				var (_, element) = CreatePannableImage();

				MouseDown(element, MouseButton.Right);

				Assert.AreSame(Cursors.Arrow, element.Cursor);
			});
		}

		[TestMethod]
		[TestCategory("Unit")]
		public void SetIsEnabled_False_StopsRespondingToTheMouse()
		{
			StaTestContext.Run(() =>
			{
				var (_, element) = CreatePannableImage();
				Pan.SetIsEnabled(element, false);

				MouseDown(element, MouseButton.Left);

				Assert.AreSame(Cursors.Arrow, element.Cursor);
			});
		}

		[TestMethod]
		[TestCategory("Unit")]
		public void GetIsEnabled_AfterSetting_RoundTrips()
		{
			StaTestContext.Run(() =>
			{
				var (_, element) = CreatePannableImage();

				Assert.IsTrue(Pan.GetIsEnabled(element));

				Pan.SetIsEnabled(element, false);

				Assert.IsFalse(Pan.GetIsEnabled(element));
			});
		}

		[TestMethod]
		[TestCategory("Unit")]
		public void SetIsEnabled_OnSomethingThatIsNotAFrameworkElement_Throws()
		{
			StaTestContext.Run(() =>
			{
				Assert.ThrowsException<NotSupportedException>(() => Pan.SetIsEnabled(new UIElement(), true));
			});
		}
	}
}
