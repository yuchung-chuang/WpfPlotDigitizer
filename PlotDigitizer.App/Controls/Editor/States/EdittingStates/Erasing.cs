using PlotDigitizer.Core;

using System;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PlotDigitizer.App
{
	public class Erasing : EdittingState
	{
		private static readonly int fps = 24;

		public override void MouseMove(Editor editor, MouseEventArgs e)
		{
			base.MouseMove(editor, e);
			var stopwatch = EditorMode.EraserMode.Stopwatch;

			var centre = e.GetPosition(editor.editCanvas);
			var size = new Vector(editor.eraserRect.ActualWidth, editor.eraserRect.ActualHeight);
			var position = centre - size / 2;
			Canvas.SetLeft(editor.eraserRect, position.X);
			Canvas.SetTop(editor.eraserRect, position.Y);
			var rect = new Rectangle(
				(int)Math.Round(position.X),
				(int)Math.Round(position.Y),
				(int)Math.Round(size.X),
				(int)Math.Round(size.Y));
			editor.Image.EraseImage(rect);
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
			if (editor.EditManager.EditCommand.CanExecute((image, "erase image")))
				editor.EditManager.EditCommand.Execute((image, "erase image"));

			editor.EdittingState = NotEditting;
		}
	}
}