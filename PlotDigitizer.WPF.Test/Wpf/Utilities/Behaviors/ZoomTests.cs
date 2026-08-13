using Microsoft.VisualStudio.TestTools.UnitTesting;

using PlotDigitizer.WPF;

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace PlotDigitizer.WPF.Tests.Utilities.Behaviors
{
	/// <summary>
	/// The zoom behaviour attached to the editor image: the wheel scales the element, never below
	/// 1, and reports the new scale back through the attached properties.
	/// </summary>
	[TestClass]
	public class ZoomTests
	{
		private static (Grid parent, Image element) CreateZoomableImage()
		{
			var element = new Image { Width = 100, Height = 100 };
			var parent = new Grid();
			parent.Children.Add(element);
			Zoom.SetIsEnabled(element, true);
			return (parent, element);
		}

		private static void Wheel(UIElement element, int delta)
			=> element.RaiseEvent(new MouseWheelEventArgs(Mouse.PrimaryDevice, 0, delta)
			{
				RoutedEvent = UIElement.MouseWheelEvent,
			});

		[TestMethod]
		[TestCategory("Unit")]
		public void SetIsEnabled_True_PreparesTransformsAndClipsTheParent()
		{
			StaTestContext.Run(() =>
			{
				var (parent, element) = CreateZoomableImage();

				Assert.IsInstanceOfType(element.RenderTransform, typeof(TransformGroup), "transforms are installed");
				Assert.AreEqual(4, ((TransformGroup)element.RenderTransform).Children.Count);
				Assert.IsTrue(parent.ClipToBounds, "the parent must clip the zoomed content");
				Assert.AreEqual(1d, Zoom.GetScale(element), "default scale");
			});
		}

		[TestMethod]
		[TestCategory("Unit")]
		public void MouseWheel_ScrollUp_ZoomsInAndReportsTheNewScale()
		{
			StaTestContext.Run(() =>
			{
				var (_, element) = CreateZoomableImage();
				double? reported = null;
				Zoom.SetMouseWheel(element, (s, scale) => reported = scale);

				Wheel(element, 120);

				Assert.AreEqual(1.2, Zoom.GetScale(element), 1e-9, "scale after one notch");
				Assert.AreEqual(1.2, reported.Value, 1e-9, "the reported scale");
			});
		}

		/// <summary>
		/// The next scale is derived from the <see cref="ScaleTransform"/>, which is animated, so
		/// a second notch in the same frame recomputes the same target instead of compounding.
		/// Once the transform holds the new scale, the next notch zooms a further 20%.
		/// </summary>
		[TestMethod]
		[TestCategory("Unit")]
		public void MouseWheel_ScrollUpTwice_CompoundsOnlyOnceTheTransformCaughtUp()
		{
			StaTestContext.Run(() =>
			{
				var (_, element) = CreateZoomableImage();
				var scale = (ScaleTransform)((TransformGroup)element.RenderTransform).Children[0];

				Wheel(element, 120);
				Wheel(element, 120);

				Assert.AreEqual(1.2, Zoom.GetScale(element), 1e-9, "two notches in the same frame");

				scale.BeginAnimation(ScaleTransform.ScaleXProperty, null);
				scale.ScaleX = 1.2;
				Wheel(element, 120);

				Assert.AreEqual(1.44, Zoom.GetScale(element), 1e-9, "a notch on top of the applied scale");
			});
		}

		[TestMethod]
		[TestCategory("Unit")]
		public void MouseWheel_ScrollDownAtMinimumScale_StaysAtOne()
		{
			StaTestContext.Run(() =>
			{
				var (_, element) = CreateZoomableImage();

				Wheel(element, -120);

				Assert.AreEqual(1d, Zoom.GetScale(element), "the element never zooms out past its natural size");
			});
		}

		[TestMethod]
		[TestCategory("Unit")]
		public void MouseWheel_GestureDoesNotMatch_IsIgnored()
		{
			StaTestContext.Run(() =>
			{
				var (_, element) = CreateZoomableImage();
				Zoom.SetGesture(element, new MouseGesture(MouseAction.LeftClick));
				var reportCount = 0;
				Zoom.SetMouseWheel(element, (s, scale) => reportCount++);

				Wheel(element, 120);

				Assert.AreEqual(1d, Zoom.GetScale(element), "scale");
				Assert.AreEqual(0, reportCount, "callback count");
			});
		}

		[TestMethod]
		[TestCategory("Unit")]
		public void SetIsEnabled_False_StopsRespondingToTheWheel()
		{
			StaTestContext.Run(() =>
			{
				var (_, element) = CreateZoomableImage();
				Zoom.SetIsEnabled(element, false);

				Wheel(element, 120);

				Assert.AreEqual(1d, Zoom.GetScale(element));
			});
		}

		[TestMethod]
		[TestCategory("Unit")]
		public void SetIsEnabled_OnSomethingThatIsNotAFrameworkElement_Throws()
		{
			StaTestContext.Run(() =>
			{
				Assert.ThrowsException<NotSupportedException>(() => Zoom.SetIsEnabled(new UIElement(), true));
			});
		}
	}
}
