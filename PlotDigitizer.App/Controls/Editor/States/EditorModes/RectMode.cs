using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PlotDigitizer.App
{
	public class RectMode : EditorMode
	{
		public Point MouseDownPos { get; set; }

		public override void MouseDown(Editor editor, MouseButtonEventArgs e)
		{
			if (editor.EdittingState != EdittingState.NotEditting) {
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
			editor.EdittingState = EdittingState.RectSelecting;
		}
	}
}