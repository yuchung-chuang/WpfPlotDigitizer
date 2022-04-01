using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace PlotDigitizer.App
{
	public abstract class EditorState : State
	{
		public override void Enter(Editor editor)
		{
			editor.EdittingState = NotEditting.Instance;
		}
	}

	public class NoMode : EditorState
	{
		public static NoMode Instance { get; } = new NoMode();
	}

	public class PolyMode : EditorState
	{
		public static PolyMode Instance { get; } = new PolyMode();
		public override void MouseDown(Editor editor, MouseButtonEventArgs e)
		{
			if (editor.EdittingState != NotEditting.Instance) {
				return;
			}
			base.MouseDown(editor, e);
			var position = e.GetPosition(editor.editCanvas);
			// add the second point as an indicator
			editor.selectPoly.Points = new PointCollection { position, position };
			editor.EdittingState = PolySelecting.Instance;
		}
	}

	public class RectMode : EditorState
	{
		public static RectMode Instance { get; } = new RectMode();
		public Point MouseDownPos { get; set; }

		public override void MouseDown(Editor editor, MouseButtonEventArgs e)
		{
			if (editor.EdittingState != NotEditting.Instance) {
				return;
			}
			base.MouseDown(editor, e);
			editor.mainGrid.CaptureMouse();
			MouseDownPos = e.GetPosition(editor.editCanvas);
			Canvas.SetLeft(editor.selectRect, MouseDownPos.X);
			Canvas.SetTop(editor.selectRect, MouseDownPos.Y);
			editor.selectRect.Width = 0;
			editor.selectRect.Height = 0;
			editor.selectRect.Focus();
			editor.EdittingState = RectSelecting.Instance;
		}
	}

	public class ErasorMode : EditorState
	{
		public static ErasorMode Instance { get; } = new ErasorMode();
		public Stopwatch Stopwatch { get; } = new Stopwatch();

		public override void MouseDown(Editor editor, MouseButtonEventArgs e)
		{
			if (editor.EdittingState != NotEditting.Instance) {
				return;
			}
			base.MouseDown(editor, e);
			editor.mainGrid.CaptureMouse();
			Stopwatch.Restart();
			editor.EdittingState = Erasing.Instance;
			Erasing.Instance.MouseMove(editor, e);
		}
	}

	public class PencilMode : EditorState
	{
		public static PencilMode Instance { get; } = new PencilMode();
		public Stopwatch Stopwatch { get; } = new Stopwatch();
		public override void MouseDown(Editor editor, MouseButtonEventArgs e)
		{
			if (editor.EdittingState != NotEditting.Instance) {
				return;
			}
			base.MouseDown(editor, e);
			editor.mainGrid.CaptureMouse();
			Stopwatch.Restart();
			editor.EdittingState = Drawing.Instance;
			Drawing.Instance.MouseMove(editor, e);
		}
	}
}