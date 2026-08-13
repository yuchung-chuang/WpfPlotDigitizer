using Emgu.CV;
using Emgu.CV.Structure;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using PlotDigitizer.Core.Tests.Fakes;
using PlotDigitizer.WPF;

using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;

namespace PlotDigitizer.WPF.Tests.Controls
{
	/// <summary>
	/// Drives the editor state machine through its public state objects. Every transition the
	/// user can trigger with the mouse or the delete key is covered, together with the edits that
	/// reach the undo stack.
	/// </summary>
	/// <remarks>
	/// The modes and editting states are process-wide singletons, so the gestures they hold are
	/// restored after each test.
	/// </remarks>
	[TestClass]
	public class EditorStateMachineTests
	{
		private MouseGesture editGesture;
		private MouseGesture selectedGesture;

		[TestInitialize]
		public void OnTestInitialize()
		{
			editGesture = EdittingState.PolySelecting.EditGesture;
			selectedGesture = EdittingState.PolySelecting.SelectedGesture;
		}

		[TestCleanup]
		public void OnTestCleanup()
		{
			EdittingState.PolySelecting.EditGesture = editGesture;
			EdittingState.PolySelecting.SelectedGesture = selectedGesture;
		}

		private static (Editor editor, FakeEditService<Image<Rgba, byte>> editService) CreateEditor()
		{
			var editService = new FakeEditService<Image<Rgba, byte>>();
			var image = new Image<Rgba, byte>(20, 20);
			editService.Initialise(image);
			var editor = new Editor
			{
				EditService = editService,
				Image = image.Copy(),
			};
			editor.Measure(new Size(200, 200));
			editor.Arrange(new Rect(0, 0, 200, 200));
			return (editor, editService);
		}

		private static MouseButtonEventArgs Click(MouseButton button = MouseButton.Left)
			=> new(Mouse.PrimaryDevice, 0, button) { RoutedEvent = UIElement.MouseDownEvent };

		private static KeyEventArgs Delete(HwndSource source)
			=> new(Keyboard.PrimaryDevice, source, 0, Key.Delete) { RoutedEvent = UIElement.KeyDownEvent };

		[TestMethod]
		[TestCategory("Unit")]
		public void Editor_Initially_HasNoModeAndIsNotEditting()
		{
			StaTestContext.Run(() =>
			{
				var (editor, _) = CreateEditor();

				Assert.AreSame(EditorMode.NoMode, editor.EditorMode, "mode");
				Assert.AreSame(EdittingState.NotEditting, editor.EdittingState, "editting state");
			});
		}

		[TestMethod]
		[TestCategory("Unit")]
		public void EditorMode_WhenSwitched_CancelsTheEditInProgress()
		{
			StaTestContext.Run(() =>
			{
				var (editor, _) = CreateEditor();
				editor.EditorMode = EditorMode.RectMode;
				EditorMode.RectMode.MouseDown(editor, Click());
				Assert.AreSame(EdittingState.RectSelecting, editor.EdittingState, "precondition");

				editor.EditorMode = EditorMode.PencilMode;

				Assert.AreSame(EdittingState.NotEditting, editor.EdittingState);
			});
		}

		[TestMethod]
		[TestCategory("Unit")]
		public void NoMode_MouseDown_StartsNothing()
		{
			StaTestContext.Run(() =>
			{
				var (editor, _) = CreateEditor();

				EditorMode.NoMode.MouseDown(editor, Click());

				Assert.AreSame(EdittingState.NotEditting, editor.EdittingState);
			});
		}

		[TestMethod]
		[TestCategory("Unit")]
		public void RectMode_MouseDownThenUp_SelectsARectangle()
		{
			StaTestContext.Run(() =>
			{
				var (editor, _) = CreateEditor();

				EditorMode.RectMode.MouseDown(editor, Click());
				Assert.AreSame(EdittingState.RectSelecting, editor.EdittingState, "while dragging");

				editor.EdittingState.MouseUp(editor, Click());

				Assert.AreSame(EdittingState.RectSelected, editor.EdittingState, "after releasing");
			});
		}

		[TestMethod]
		[TestCategory("Unit")]
		public void RectMode_MouseDownWhileAlreadySelecting_IsIgnored()
		{
			StaTestContext.Run(() =>
			{
				var (editor, _) = CreateEditor();
				editor.EdittingState = EdittingState.Erasing;

				EditorMode.RectMode.MouseDown(editor, Click());

				Assert.AreSame(EdittingState.Erasing, editor.EdittingState);
			});
		}

		[TestMethod]
		[TestCategory("Unit")]
		public void RectSelected_MouseDown_DiscardsTheSelectionAndStartsANewOne()
		{
			StaTestContext.Run(() =>
			{
				var (editor, _) = CreateEditor();
				editor.EditorMode = EditorMode.RectMode;
				editor.EdittingState = EdittingState.RectSelected;

				editor.EdittingState.MouseDown(editor, Click());

				Assert.AreSame(EdittingState.RectSelecting, editor.EdittingState);
			});
		}

		[TestMethod]
		[TestCategory("Unit")]
		public void RectSelected_Delete_ErasesTheRegionAndPushesOneEdit()
		{
			StaTestContext.Run(() =>
			{
				var (editor, editService) = CreateEditor();
				using var source = new HwndSource(new HwndSourceParameters("PlotDigitizer tests"));
				editor.EditorMode = EditorMode.RectMode;
				EditorMode.RectMode.MouseDown(editor, Click());
				editor.EdittingState.MouseUp(editor, Click());

				editor.EdittingState.KeyDown(editor, Delete(source));

				Assert.AreEqual(1, editService.Edits.Count, "edit count");
				Assert.AreEqual("Delete rectangle region", editService.Edits.Single().tag, "tag");
				Assert.AreSame(EdittingState.NotEditting, editor.EdittingState, "editting state");
			});
		}

		[TestMethod]
		[TestCategory("Unit")]
		public void RectSelected_DeleteWhenTheEditIsRejected_LeavesTheUndoStackAlone()
		{
			StaTestContext.Run(() =>
			{
				var (editor, editService) = CreateEditor();
				using var source = new HwndSource(new HwndSourceParameters("PlotDigitizer tests"));
				editService.CanEditResult = false;
				editor.EdittingState = EdittingState.RectSelected;

				editor.EdittingState.KeyDown(editor, Delete(source));

				Assert.AreEqual(0, editService.Edits.Count, "edit count");
				Assert.AreSame(EdittingState.NotEditting, editor.EdittingState, "editting state");
			});
		}

		[TestMethod]
		[TestCategory("Unit")]
		public void PencilMode_MouseDownThenUp_DrawsAndPushesOneEdit()
		{
			StaTestContext.Run(() =>
			{
				var (editor, editService) = CreateEditor();

				EditorMode.PencilMode.MouseDown(editor, Click());
				Assert.AreSame(EdittingState.Drawing, editor.EdittingState, "while drawing");

				editor.EdittingState.MouseUp(editor, Click());

				Assert.AreSame(EdittingState.NotEditting, editor.EdittingState, "after releasing");
				Assert.AreEqual("draw image", editService.Edits.Single().tag, "tag");
			});
		}

		[TestMethod]
		[TestCategory("Unit")]
		public void EraserMode_MouseDownThenUp_ErasesAndPushesOneEdit()
		{
			StaTestContext.Run(() =>
			{
				var (editor, editService) = CreateEditor();

				EditorMode.EraserMode.MouseDown(editor, Click());
				Assert.AreSame(EdittingState.Erasing, editor.EdittingState, "while erasing");

				editor.EdittingState.MouseUp(editor, Click());

				Assert.AreSame(EdittingState.NotEditting, editor.EdittingState, "after releasing");
				Assert.AreEqual("erase image", editService.Edits.Single().tag, "tag");
			});
		}

		[TestMethod]
		[TestCategory("Unit")]
		public void PolyMode_MouseDown_StartsSelectingAPolygon()
		{
			StaTestContext.Run(() =>
			{
				var (editor, _) = CreateEditor();

				EditorMode.PolyMode.MouseDown(editor, Click());

				Assert.AreSame(EdittingState.PolySelecting, editor.EdittingState);
			});
		}

		[TestMethod]
		[TestCategory("Unit")]
		public void PolySelecting_EditGesture_KeepsCollectingPoints()
		{
			StaTestContext.Run(() =>
			{
				var (editor, _) = CreateEditor();
				EdittingState.PolySelecting.EditGesture = new MouseGesture(MouseAction.LeftClick);
				EdittingState.PolySelecting.SelectedGesture = new MouseGesture(MouseAction.RightClick);
				EditorMode.PolyMode.MouseDown(editor, Click());

				editor.EdittingState.MouseDown(editor, Click());

				Assert.AreSame(EdittingState.PolySelecting, editor.EdittingState);
			});
		}

		[TestMethod]
		[TestCategory("Unit")]
		public void PolySelecting_ClosingGesture_ClosesThePolygon()
		{
			StaTestContext.Run(() =>
			{
				var (editor, _) = CreateEditor();
				EdittingState.PolySelecting.EditGesture = new MouseGesture(MouseAction.LeftClick);
				EdittingState.PolySelecting.SelectedGesture = new MouseGesture(MouseAction.RightClick);
				EditorMode.PolyMode.MouseDown(editor, Click());

				editor.EdittingState.MouseDown(editor, Click(MouseButton.Right));

				Assert.AreSame(EdittingState.PolySelected, editor.EdittingState);
			});
		}

		[TestMethod]
		[TestCategory("Unit")]
		public void PolySelected_Delete_ErasesTheRegionAndPushesOneEdit()
		{
			StaTestContext.Run(() =>
			{
				var (editor, editService) = CreateEditor();
				using var source = new HwndSource(new HwndSourceParameters("PlotDigitizer tests"));
				EdittingState.PolySelecting.EditGesture = new MouseGesture(MouseAction.LeftClick);
				EdittingState.PolySelecting.SelectedGesture = new MouseGesture(MouseAction.RightClick);
				EditorMode.PolyMode.MouseDown(editor, Click());
				editor.EdittingState.MouseDown(editor, Click(MouseButton.Right));

				editor.EdittingState.KeyDown(editor, Delete(source));

				Assert.AreEqual("Delete polygon region", editService.Edits.Single().tag, "tag");
				Assert.AreSame(EdittingState.NotEditting, editor.EdittingState, "editting state");
			});
		}

		[TestMethod]
		[TestCategory("Unit")]
		public void PolySelected_MouseDown_DiscardsTheSelectionAndStartsANewOne()
		{
			StaTestContext.Run(() =>
			{
				var (editor, _) = CreateEditor();
				editor.EditorMode = EditorMode.PolyMode;
				editor.EdittingState = EdittingState.PolySelected;

				editor.EdittingState.MouseDown(editor, Click());

				Assert.AreSame(EdittingState.PolySelecting, editor.EdittingState);
			});
		}

		[TestMethod]
		[TestCategory("Unit")]
		public void NotEditting_Delete_DoesNothing()
		{
			StaTestContext.Run(() =>
			{
				var (editor, editService) = CreateEditor();
				using var source = new HwndSource(new HwndSourceParameters("PlotDigitizer tests"));

				editor.EdittingState.KeyDown(editor, Delete(source));

				Assert.AreEqual(0, editService.Edits.Count, "edit count");
				Assert.AreSame(EdittingState.NotEditting, editor.EdittingState, "editting state");
			});
		}

		[TestMethod]
		[TestCategory("Unit")]
		public void ImageSource_FollowsTheImage()
		{
			StaTestContext.Run(() =>
			{
				var (editor, _) = CreateEditor();

				Assert.IsNotNull(editor.ImageSource, "an image produces a source");

				editor.Image = null;

				Assert.IsNull(editor.ImageSource, "no image produces no source");
			});
		}
	}
}
