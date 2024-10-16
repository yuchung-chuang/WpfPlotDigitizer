﻿using PlotDigitizer.Core;

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PlotDigitizer.WPF
{
	public class Drawing : EdittingState
	{
		private static readonly int fps = 24;

		public override void MouseMove(Editor editor, MouseEventArgs e)
		{
			base.MouseMove(editor, e);
			var stopwatch = EditorMode.PencilMode.Stopwatch;

			var centre = e.GetPosition(editor.editCanvas);
			var size = new Vector(editor.pencilPointer.ActualWidth, editor.pencilPointer.ActualHeight);
			var position = centre - size / 2;
			Canvas.SetLeft(editor.pencilPointer, position.X);
			Canvas.SetTop(editor.pencilPointer, position.Y);

			var centre2 = new System.Drawing.Point(
				(int)Math.Round(centre.X),
				(int)Math.Round(centre.Y));

			editor.Image.DrawCircle(centre2, (int)size.X / 2);

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
			var image = editor.Image.Copy(); // save a copy to EditService
			if (editor.EditService.CanEdit((image, "draw image")))
				editor.EditService.Edit((image, "draw image"));

			editor.EdittingState = NotEditting;
		}
	}
}