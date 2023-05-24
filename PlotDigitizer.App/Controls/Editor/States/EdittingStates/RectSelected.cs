﻿using PlotDigitizer.Core;

using System;
using System.Drawing;
using System.Windows.Controls;
using System.Windows.Input;

namespace PlotDigitizer.App
{
	public class RectSelected : EdittingState
	{
		public override void MouseDown(Editor editor, MouseButtonEventArgs e)
		{
			base.MouseDown(editor, e);
			editor.EdittingState = NotEditting;
			EditorState.RectMode.MouseDown(editor, e);
		}

		public override void KeyDown(Editor editor, KeyEventArgs e)
		{
			base.KeyDown(editor, e);
			var selectRect = editor.selectRect;
			var Image = editor.Image;
			var EditManager = editor.EditManager;

			var left = Canvas.GetLeft(selectRect);
			var top = Canvas.GetTop(selectRect);
			var rect = new Rectangle(
				(int)Math.Round(left),
				(int)Math.Round(top),
				(int)Math.Round(selectRect.Width),
				(int)Math.Round(selectRect.Height));
			Methods.EraseImage(Image, rect);
			var image = Image.Copy();
			if (EditManager.EditCommand.CanExecute((image, "Delete rectangle region"))) {
				EditManager.EditCommand.Execute((image, "Delete rectangle region"));
			}
			editor.EdittingState = NotEditting;
		}
	}
}