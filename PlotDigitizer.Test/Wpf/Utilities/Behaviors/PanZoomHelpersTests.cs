using Microsoft.VisualStudio.TestTools.UnitTesting;

using PlotDigitizer.WPF;

using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace PlotDigitizer.WPF.Tests.Utilities.Behaviors
{
	/// <summary>
	/// The pan and zoom behaviours both rely on every element carrying the exact same transform
	/// layout, so that a cached transform can be interpreted by either behaviour.
	/// </summary>
	[TestClass]
	public class PanZoomHelpersTests
	{
		[TestMethod]
		[TestCategory("Unit")]
		public void EnsureTransforms_ElementWithoutTransforms_InstallsScaleTranslateRotateSkewInOrder()
		{
			StaTestContext.Run(() =>
			{
				var element = new Image();

				element.EnsureTransforms();

				var children = ((TransformGroup)element.RenderTransform).Children;
				Assert.AreEqual(4, children.Count, "transform count");
				Assert.IsInstanceOfType(children[0], typeof(ScaleTransform), "scale must come first");
				Assert.IsInstanceOfType(children[1], typeof(TranslateTransform), "translate must come second");
				Assert.IsInstanceOfType(children[2], typeof(RotateTransform));
				Assert.IsInstanceOfType(children[3], typeof(SkewTransform));
				Assert.AreEqual(new Point(0, 0), element.RenderTransformOrigin, "origin");
			});
		}

		[TestMethod]
		[TestCategory("Unit")]
		public void EnsureTransforms_CalledTwice_KeepsTheExistingTransforms()
		{
			StaTestContext.Run(() =>
			{
				var element = new Image();
				element.EnsureTransforms();
				var first = element.RenderTransform;
				((TransformGroup)first).Children.OfType<ScaleTransform>().Single().ScaleX = 3;

				element.EnsureTransforms();

				Assert.AreSame(first, element.RenderTransform, "the transform group must be reused");
				Assert.AreEqual(3, ((TransformGroup)element.RenderTransform).Children.OfType<ScaleTransform>().Single().ScaleX);
			});
		}

		[TestMethod]
		[TestCategory("Unit")]
		public void EnsureTransforms_WrongTransformLayout_IsReplaced()
		{
			StaTestContext.Run(() =>
			{
				var element = new Image
				{
					// right type, wrong content
					RenderTransform = new TransformGroup { Children = [new TranslateTransform(), new ScaleTransform()] },
				};

				element.EnsureTransforms();

				var children = ((TransformGroup)element.RenderTransform).Children;
				Assert.AreEqual(4, children.Count, "transform count");
				Assert.IsInstanceOfType(children[0], typeof(ScaleTransform));
			});
		}

		[TestMethod]
		[TestCategory("Unit")]
		public void GetAbsolutePosition_Always_RestoresTheTransformsItTemporarilyCleared()
		{
			StaTestContext.Run(() =>
			{
				var element = new Image();
				element.EnsureTransforms();
				var children = ((TransformGroup)element.RenderTransform).Children;
				var args = new MouseEventArgs(Mouse.PrimaryDevice, 0);

				var position = args.GetAbsolutePosition(element);

				Assert.AreSame(children, ((TransformGroup)element.RenderTransform).Children, "transforms must be put back");
				Assert.AreEqual(4, ((TransformGroup)element.RenderTransform).Children.Count);
				// an element outside a presentation source reports the origin
				Assert.AreEqual(new Point(0, 0), position);
			});
		}

		[TestMethod]
		[TestCategory("Unit")]
		public void BeginAnimation_SupportedTargetTypes_StartAnAnimation()
		{
			StaTestContext.Run(() =>
			{
				var translate = new TranslateTransform();
				var rotate = new RotateTransform();
				var brush = new SolidColorBrush(Colors.Black);
				var border = new Border();

				translate.BeginAnimation(TranslateTransform.XProperty, 10d, 1);
				rotate.BeginAnimation(RotateTransform.AngleProperty, 45d, 1);
				brush.BeginAnimation(SolidColorBrush.ColorProperty, Colors.Red, 1);
				border.BeginAnimation(FrameworkElement.MarginProperty, new Thickness(5), 1);

				Assert.IsTrue(translate.HasAnimatedProperties, "double animation");
				Assert.IsTrue(rotate.HasAnimatedProperties, "double animation on another target");
				Assert.IsTrue(brush.HasAnimatedProperties, "colour animation");
				Assert.IsTrue(border.HasAnimatedProperties, "thickness animation");
			});
		}

		[TestMethod]
		[TestCategory("Unit")]
		public void BeginAnimation_UnsupportedTargetType_Throws()
		{
			StaTestContext.Run(() =>
			{
				var translate = new TranslateTransform();

				Assert.ThrowsException<NotSupportedException>(
					() => translate.BeginAnimation(TranslateTransform.XProperty, "10", 1));
			});
		}
	}
}
