using Microsoft.VisualStudio.TestTools.UnitTesting;

using PlotDigitizer.WPF;

using System.Windows;
using System.Windows.Input;

namespace PlotDigitizer.WPF.Tests.Controls
{
	/// <summary>
	/// Drives the selection box state machine directly. The control is laid out first so that
	/// <c>ActualWidth</c>/<c>ActualHeight</c> clamp the adjustments, and every mouse position
	/// resolves to the origin because the box is not hosted in a window.
	/// </summary>
	[TestClass]
	public class SelectionBoxStateMachineTests
	{
		private static SelectionBox CreateArrangedBox()
		{
			var box = new SelectionBox
			{
				BoxRect = new Rect(10, 20, 100, 50),
				BoxThickness = 2,
			};
			box.Measure(new Size(200, 200));
			box.Arrange(new Rect(0, 0, 200, 200));
			return box;
		}

		private static MouseButtonEventArgs MouseButton(MouseButton button = System.Windows.Input.MouseButton.Left)
			=> new(Mouse.PrimaryDevice, 0, button) { RoutedEvent = UIElement.MouseDownEvent };

		private static MouseEventArgs MouseMove() => new(Mouse.PrimaryDevice, 0) { RoutedEvent = UIElement.MouseMoveEvent };

		[TestMethod]
		[TestCategory("Unit")]
		public void State_OnAFreshBox_IsNoState()
		{
			StaTestContext.Run(() => Assert.AreSame(SelectionBoxStates.None, CreateArrangedBox().State));
		}

		[TestMethod]
		[TestCategory("Unit")]
		public void NullState_MouseDownOnACorner_EntersAdjustAndSwallowsTheClick()
		{
			StaTestContext.Run(() =>
			{
				var box = CreateArrangedBox();
				box.BoxRect = new Rect(0, 0, 100, 100); // the top left corner is under the mouse
				var e = MouseButton();

				box.State.MouseDown(box, e);

				var adjust = box.State as AdjustState;
				Assert.IsNotNull(adjust, $"expected an adjust state, got {box.State}");
				Assert.IsInstanceOfType(adjust.StateX, typeof(AdjustLeftState), "horizontal");
				Assert.IsInstanceOfType(adjust.StateY, typeof(AdjustTopState), "vertical");
				Assert.IsTrue(e.Handled, "the click must not reach the controls underneath");
				Assert.AreSame(Cursors.SizeNWSE, box.Cursor, "cursor");
			});
		}

		[TestMethod]
		[TestCategory("Unit")]
		public void NullState_MouseDownAwayFromTheBox_StaysInNoState()
		{
			StaTestContext.Run(() =>
			{
				var box = CreateArrangedBox(); // the box starts at (10, 20), the mouse is at the origin
				var e = MouseButton();

				box.State.MouseDown(box, e);

				Assert.AreSame(SelectionBoxStates.None, box.State, "state");
				Assert.IsFalse(e.Handled, "the click must pass through");
			});
		}

		[TestMethod]
		[TestCategory("Unit")]
		public void NullState_MouseDownWithAnotherButton_IsIgnored()
		{
			StaTestContext.Run(() =>
			{
				var box = CreateArrangedBox();
				box.BoxRect = new Rect(0, 0, 100, 100);
				var e = MouseButton(System.Windows.Input.MouseButton.Right);

				box.State.MouseDown(box, e);

				Assert.AreSame(SelectionBoxStates.None, box.State, "state");
				Assert.IsFalse(e.Handled, "the click must pass through");
			});
		}

		[TestMethod]
		[TestCategory("Unit")]
		public void NullState_MouseMove_UpdatesTheCursorButNotTheState()
		{
			StaTestContext.Run(() =>
			{
				var box = CreateArrangedBox();
				box.BoxRect = new Rect(0, 0, 100, 100);

				box.State.MouseMove(box, MouseMove());

				Assert.AreSame(SelectionBoxStates.None, box.State, "state");
				Assert.AreSame(Cursors.SizeNWSE, box.Cursor, "cursor");
			});
		}

		[TestMethod]
		[TestCategory("Unit")]
		public void AdjustLeft_MouseMove_DragsTheLeftEdgeAndKeepsTheRightOne()
		{
			StaTestContext.Run(() =>
			{
				var box = CreateArrangedBox();

				SelectionBoxStates.AdjustLeft.MouseMove(box, MouseMove());

				Assert.AreEqual(0, box.BoxLeft, "left follows the mouse");
				Assert.AreEqual(110, box.BoxRight, "right is unchanged");
			});
		}

		[TestMethod]
		[TestCategory("Unit")]
		public void AdjustTop_MouseMove_DragsTheTopEdgeAndKeepsTheBottomOne()
		{
			StaTestContext.Run(() =>
			{
				var box = CreateArrangedBox();

				SelectionBoxStates.AdjustTop.MouseMove(box, MouseMove());

				Assert.AreEqual(0, box.BoxTop, "top follows the mouse");
				Assert.AreEqual(70, box.BoxBottom, "bottom is unchanged");
			});
		}

		[TestMethod]
		[TestCategory("Unit")]
		public void AdjustRight_MouseMoveBeyondTheLeftEdge_ClampsToAOnePixelWideBox()
		{
			StaTestContext.Run(() =>
			{
				var box = CreateArrangedBox();

				SelectionBoxStates.AdjustRight.MouseMove(box, MouseMove());

				Assert.AreEqual(10, box.BoxLeft, "left is unchanged");
				Assert.AreEqual(1, box.BoxWidth, "the box cannot be collapsed");
			});
		}

		[TestMethod]
		[TestCategory("Unit")]
		public void AdjustBottom_MouseMoveAboveTheTopEdge_ClampsToAOnePixelTallBox()
		{
			StaTestContext.Run(() =>
			{
				var box = CreateArrangedBox();

				SelectionBoxStates.AdjustBottom.MouseMove(box, MouseMove());

				Assert.AreEqual(20, box.BoxTop, "top is unchanged");
				Assert.AreEqual(1, box.BoxHeight, "the box cannot be collapsed");
			});
		}

		[TestMethod]
		[TestCategory("Unit")]
		public void AdjustState_MouseMove_AppliesBothAxes()
		{
			StaTestContext.Run(() =>
			{
				var box = CreateArrangedBox();
				var state = new AdjustState
				{
					StateX = SelectionBoxStates.AdjustLeft,
					StateY = SelectionBoxStates.AdjustTop,
				};

				state.MouseMove(box, MouseMove());

				Assert.AreEqual(new Rect(0, 0, 110, 70), box.BoxRect);
			});
		}

		[TestMethod]
		[TestCategory("Unit")]
		public void MoveState_MouseMove_MovesTheBoxByTheMouseDelta()
		{
			StaTestContext.Run(() =>
			{
				var box = CreateArrangedBox();
				// the box was grabbed at (30, 40) while sitting at (10, 20); the mouse is now at the origin
				var state = new MoveState(new Point(10, 20), new Point(30, 40));

				state.MouseMove(box, MouseMove());

				Assert.AreEqual(new Rect(-20, -20, 100, 50), box.BoxRect, "the size must not change");
			});
		}

		[TestMethod]
		[TestCategory("Unit")]
		public void MouseUp_FromAnyState_ReturnsToNoState()
		{
			StaTestContext.Run(() =>
			{
				var box = CreateArrangedBox();
				box.State = new AdjustState { StateX = SelectionBoxStates.AdjustLeft };

				box.State.MouseUp(box, MouseButton());

				Assert.AreSame(SelectionBoxStates.None, box.State);
			});
		}

		[TestMethod]
		[TestCategory("Unit")]
		public void MouseUp_AfterAMove_ReturnsToNoState()
		{
			StaTestContext.Run(() =>
			{
				var box = CreateArrangedBox();
				box.State = new MoveState(new Point(10, 20), new Point(30, 40));

				box.State.MouseUp(box, MouseButton());

				Assert.AreSame(SelectionBoxStates.None, box.State);
			});
		}
	}
}
