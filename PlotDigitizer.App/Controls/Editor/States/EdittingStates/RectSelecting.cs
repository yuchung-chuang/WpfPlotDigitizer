﻿using System;
using System.Drawing;
using System.Windows.Controls;
using System.Windows.Input;

namespace PlotDigitizer.App
{
	public class RectSelecting : EdittingState
	{
		public override void MouseMove(Editor editor, MouseEventArgs e)
		{
			base.MouseMove(editor, e);
			var selectRect = editor.selectRect;
			var Image = editor.Image;
			var mouseDownPos = EditorState.RectMode.MouseDownPos;
			var position = e.GetPosition(editor.editCanvas);
			if (position.X < 0) {
				Canvas.SetLeft(selectRect, 0);
				selectRect.Width = mouseDownPos.X;
			}
			else if (position.X > Image.Width) {
				Canvas.SetLeft(selectRect, mouseDownPos.X);
				selectRect.Width = Image.Width - mouseDownPos.X;
			}
			else {
				var dx = position.X - mouseDownPos.X;
				if (dx < 0)
					Canvas.SetLeft(selectRect, position.X);
				else
					Canvas.SetLeft(selectRect, mouseDownPos.X);
				selectRect.Width = Math.Abs(dx);
			}

			if (position.Y < 0) {
				Canvas.SetTop(selectRect, 0);
				selectRect.Height = mouseDownPos.Y;
			}
			else if (position.Y > Image.Height) {
				Canvas.SetTop(selectRect, mouseDownPos.Y);
				selectRect.Height = Image.Height - mouseDownPos.Y;
			}
			else {
				var dy = position.Y - mouseDownPos.Y;
				if (dy < 0)
					Canvas.SetTop(selectRect, position.Y);
				else
					Canvas.SetTop(selectRect, mouseDownPos.Y);
				selectRect.Height = Math.Abs(dy);
			}

		}

		public override void MouseUp(Editor editor, MouseButtonEventArgs e)
		{
			base.MouseUp(editor, e);
			editor.mainGrid.ReleaseMouseCapture();
			editor.EdittingState = RectSelected;
		}
	}
}