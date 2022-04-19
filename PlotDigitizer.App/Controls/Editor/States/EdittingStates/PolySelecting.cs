using System;
using System.Windows.Input;

namespace PlotDigitizer.App
{
	public class PolySelecting : EdittingState
	{
		public static PolySelecting Instance { get; } = new PolySelecting();
		public override void MouseDown(Editor editor, MouseButtonEventArgs e)
		{
			base.MouseDown(editor, e);
			var editCanvas = editor.editCanvas;
			var selectPoly = editor.selectPoly;

			var position = e.GetPosition(editCanvas);
			if (e.ClickCount == 1) {
				selectPoly.Points[^1] = position;
				selectPoly.Points.Add(position);
			}
			else if (e.ClickCount == 2) {
				selectPoly.Points.Add(selectPoly.Points[0]);
				editor.EdittingState = PolySelected.Instance;
			}
			else {
				throw new Exception();
			}
		}

		public override void MouseMove(Editor editor, MouseEventArgs e)
		{
			base.MouseMove(editor, e);
			var position = e.GetPosition(editor.editCanvas);
			editor.selectPoly.Points[^1] = position;
		}
	}
}