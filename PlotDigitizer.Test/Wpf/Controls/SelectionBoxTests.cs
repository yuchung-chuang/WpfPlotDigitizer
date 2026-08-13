using Microsoft.VisualStudio.TestTools.UnitTesting;

using PlotDigitizer.WPF;

using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

namespace PlotDigitizer.WPF.Tests.Controls
{
	/// <summary>
	/// The selection box overlays the plot image, so the hit test that turns a mouse position
	/// into an adjust/move state, and the cursor that advertises it, are the whole contract of
	/// the control.
	/// </summary>
	[TestClass]
	public class SelectionBoxTests
	{
		/// <summary>Left 10, Top 20, Right 110, Bottom 70, edge tolerance 2.</summary>
		private static SelectionBox CreateBox()
			=> new()
			{
				BoxRect = new Rect(10, 20, 100, 50),
				BoxThickness = 2,
			};

		[TestMethod]
		[TestCategory("Unit")]
		public void BoxRect_WhenSet_DerivesEveryEdgeProperty()
		{
			StaTestContext.Run(() =>
			{
				var box = CreateBox();

				Assert.AreEqual(10, box.BoxLeft, "left");
				Assert.AreEqual(20, box.BoxTop, "top");
				Assert.AreEqual(100, box.BoxWidth, "width");
				Assert.AreEqual(50, box.BoxHeight, "height");
				Assert.AreEqual(110, box.BoxRight, "right");
				Assert.AreEqual(70, box.BoxBottom, "bottom");
				Assert.AreEqual(new Point(10, 20), box.BoxLocation, "location");
				Assert.AreEqual(new Thickness(10, 20, 0, 0), box.BoxMargin, "margin");
			});
		}

		[TestMethod]
		[TestCategory("Unit")]
		public void BoxLeft_WhenSet_MovesTheEdgeWithoutTouchingTheOthers()
		{
			StaTestContext.Run(() =>
			{
				var box = CreateBox();

				box.BoxLeft = 30;

				Assert.AreEqual(new Rect(30, 20, 100, 50), box.BoxRect);
			});
		}

		[TestMethod]
		[TestCategory("Unit")]
		public void BoxHeight_WhenSet_MovesTheEdgeWithoutTouchingTheOthers()
		{
			StaTestContext.Run(() =>
			{
				var box = CreateBox();

				box.BoxHeight = 5;

				Assert.AreEqual(new Rect(10, 20, 100, 5), box.BoxRect);
			});
		}

		[TestMethod]
		[TestCategory("Unit")]
		public void BoxRect_WhenSet_NotifiesEveryDerivedProperty()
		{
			StaTestContext.Run(() =>
			{
				var box = CreateBox();
				var notified = new List<string>();
				box.PropertyChanged += (s, e) => notified.Add(e.PropertyName);

				box.BoxRect = new Rect(0, 0, 10, 10);

				CollectionAssert.IsSubsetOf(
					new[] { "BoxLeft", "BoxTop", "BoxWidth", "BoxHeight", "BoxRight", "BoxBottom", "BoxMargin", "BoxLocation" },
					notified);
			});
		}

		[DataTestMethod]
		[DataRow(10.0, 45.0, "AdjustLeft", "None", DisplayName = "left edge")]
		[DataRow(110.0, 45.0, "AdjustRight", "None", DisplayName = "right edge")]
		[DataRow(60.0, 20.0, "None", "AdjustTop", DisplayName = "top edge")]
		[DataRow(60.0, 70.0, "None", "AdjustBottom", DisplayName = "bottom edge")]
		[DataRow(10.0, 20.0, "AdjustLeft", "AdjustTop", DisplayName = "top left corner")]
		[DataRow(110.0, 70.0, "AdjustRight", "AdjustBottom", DisplayName = "bottom right corner")]
		[DataRow(110.0, 20.0, "AdjustRight", "AdjustTop", DisplayName = "top right corner")]
		[DataRow(10.0, 70.0, "AdjustLeft", "AdjustBottom", DisplayName = "bottom left corner")]
		[DataRow(11.9, 45.0, "AdjustLeft", "None", DisplayName = "just inside the edge tolerance")]
		[TestCategory("Unit")]
		public void GetState_OnAnEdge_ReturnsTheMatchingAdjustState(double x, double y, string expectedX, string expectedY)
		{
			StaTestContext.Run(() =>
			{
				var box = CreateBox();

				var state = box.GetState(new Point(x, y));

				var adjust = state as AdjustState;
				Assert.IsNotNull(adjust, $"expected an adjust state, got {state}");
				Assert.AreEqual(expectedX, Name(adjust.StateX), "horizontal");
				Assert.AreEqual(expectedY, Name(adjust.StateY), "vertical");
			});

			static string Name(SelectionBoxState state) => state switch
			{
				AdjustLeftState => "AdjustLeft",
				AdjustRightState => "AdjustRight",
				AdjustTopState => "AdjustTop",
				AdjustBottomState => "AdjustBottom",
				_ => "None",
			};
		}

		[TestMethod]
		[TestCategory("Unit")]
		public void GetState_WellInsideTheBox_ReturnsAMoveState()
		{
			StaTestContext.Run(() =>
			{
				var box = CreateBox();

				Assert.IsInstanceOfType(box.GetState(new Point(60, 45)), typeof(MoveState));
			});
		}

		[TestMethod]
		[TestCategory("Unit")]
		public void GetState_InsideButMoveDisabled_ReturnsNoState()
		{
			StaTestContext.Run(() =>
			{
				var box = CreateBox();
				box.IsMoveEnabled = false;

				Assert.IsInstanceOfType(box.GetState(new Point(60, 45)), typeof(NullState));
			});
		}

		[DataTestMethod]
		[DataRow(200.0, 200.0, DisplayName = "outside the box")]
		[DataRow(10.0, 100.0, DisplayName = "on the left edge line but below the box")]
		[DataRow(60.0, 200.0, DisplayName = "under the box")]
		[TestCategory("Unit")]
		public void GetState_AwayFromTheBox_ReturnsNoState(double x, double y)
		{
			StaTestContext.Run(() =>
			{
				var box = CreateBox();

				Assert.IsInstanceOfType(box.GetState(new Point(x, y)), typeof(NullState));
			});
		}

		/// <summary>
		/// When the box is thinner than its own grab tolerance both edges match; the right edge
		/// is evaluated last and wins.
		/// </summary>
		[TestMethod]
		[TestCategory("Unit")]
		public void GetState_BoxNarrowerThanTheGrabTolerance_PrefersTheRightEdge()
		{
			StaTestContext.Run(() =>
			{
				var box = CreateBox();
				box.BoxRect = new Rect(10, 20, 1, 50);

				var adjust = (AdjustState)box.GetState(new Point(10, 45));

				Assert.IsInstanceOfType(adjust.StateX, typeof(AdjustRightState));
			});
		}

		[TestMethod]
		[TestCategory("Unit")]
		public void UpdateCursor_EachState_ShowsTheMatchingCursor()
		{
			StaTestContext.Run(() =>
			{
				var box = CreateBox();

				AssertCursor(box, SelectionBoxStates.None, Cursors.Arrow, "no state");
				AssertCursor(box, MoveState.Empty, Cursors.SizeAll, "move");
				AssertCursor(box, Adjust(SelectionBoxStates.AdjustLeft, SelectionBoxStates.None), Cursors.SizeWE, "left");
				AssertCursor(box, Adjust(SelectionBoxStates.AdjustRight, SelectionBoxStates.None), Cursors.SizeWE, "right");
				AssertCursor(box, Adjust(SelectionBoxStates.None, SelectionBoxStates.AdjustTop), Cursors.SizeNS, "top");
				AssertCursor(box, Adjust(SelectionBoxStates.None, SelectionBoxStates.AdjustBottom), Cursors.SizeNS, "bottom");
				AssertCursor(box, Adjust(SelectionBoxStates.AdjustLeft, SelectionBoxStates.AdjustTop), Cursors.SizeNWSE, "top left");
				AssertCursor(box, Adjust(SelectionBoxStates.AdjustRight, SelectionBoxStates.AdjustBottom), Cursors.SizeNWSE, "bottom right");
				AssertCursor(box, Adjust(SelectionBoxStates.AdjustRight, SelectionBoxStates.AdjustTop), Cursors.SizeNESW, "top right");
				AssertCursor(box, Adjust(SelectionBoxStates.AdjustLeft, SelectionBoxStates.AdjustBottom), Cursors.SizeNESW, "bottom left");
			});

			static AdjustState Adjust(SelectionBoxState x, SelectionBoxState y) => new() { StateX = x, StateY = y };

			static void AssertCursor(SelectionBox box, SelectionBoxState state, Cursor expected, string message)
			{
				box.Cursor = null;
				box.UpdateCursor(state);
				Assert.AreSame(expected, box.Cursor, message);
			}
		}
	}
}
