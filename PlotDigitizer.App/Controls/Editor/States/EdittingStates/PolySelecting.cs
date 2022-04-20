using System;
using System.Windows.Input;

namespace PlotDigitizer.App
{
	public class PolySelecting : EdittingState
	{
		public MouseGesture EditGesture { get; set; } = new MouseGesture(MouseAction.LeftClick);
		public MouseGesture SelectedGesture { get; set; } = new MouseGesture(MouseAction.LeftDoubleClick);
		public override void MouseDown(Editor editor, MouseButtonEventArgs e)
		{
			base.MouseDown(editor, e);
			var editCanvas = editor.editCanvas;
			var selectPoly = editor.selectPoly;

			var position = e.GetPosition(editCanvas);
			if (EditGesture.Matches(editor,e)) {
				selectPoly.Points[^1] = position;
				selectPoly.Points.Add(position);
			}
			else if (SelectedGesture.Matches(editor,e)) {
				selectPoly.Points.Add(selectPoly.Points[0]);
				editor.EdittingState = PolySelected;
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