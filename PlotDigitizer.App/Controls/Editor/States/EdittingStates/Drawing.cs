using PlotDigitizer.Core;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PlotDigitizer.App
{
	public class Drawing : EdittingState
	{
		private static readonly int fps = 24;
		public static Drawing Instance { get; } = new Drawing();
		public override void MouseMove(Editor editor, MouseEventArgs e)
		{
			base.MouseMove(editor, e);
			var stopwatch = PencilMode.Instance.Stopwatch;

			var centre = e.GetPosition(editor.editCanvas);
			var size = new Vector(editor.pencilPointer.ActualWidth, editor.pencilPointer.ActualHeight);
			var position = centre - size / 2;
			Canvas.SetLeft(editor.pencilPointer, position.X);
			Canvas.SetTop(editor.pencilPointer, position.Y);

			var centre2 = new System.Drawing.Point(
				(int)Math.Round(centre.X),
				(int)Math.Round(centre.Y));

			Methods.DrawImage(editor.Image, centre2, (int)size.X / 2);

			// update the image by "N" frames per second
			if (stopwatch.ElapsedMilliseconds > 1000 / fps) {
				editor.OnPropertyChanged(nameof(editor.ImageSource));
				stopwatch.Restart();
			}
		}

		public override void MouseUp(Editor editor, MouseButtonEventArgs e)
		{
			base.MouseUp(editor, e);

			editor.mainGrid.ReleaseMouseCapture();
			var image = editor.Image.Copy(); // save a copy to editManager
			if (editor.EditManager.EditCommand.CanExecute((image, "draw image")))
				editor.EditManager.EditCommand.Execute((image, "draw image"));

			editor.EdittingState = NotEditting.Instance;
		}
	}
}