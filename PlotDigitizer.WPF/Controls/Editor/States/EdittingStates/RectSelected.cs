﻿using PlotDigitizer.Core;

using System;
using System.Drawing;
using System.Windows.Controls;
using System.Windows.Input;

namespace PlotDigitizer.WPF
{
	public class RectSelected : EdittingState
	{
		public override void MouseDown(Editor editor, MouseButtonEventArgs e)
		{
			base.MouseDown(editor, e);
			editor.EdittingState = NotEditting;
			EditorMode.RectMode.MouseDown(editor, e);
		}

		public override void KeyDown(Editor editor, KeyEventArgs e)
		{
			base.KeyDown(editor, e);
			var selectRect = editor.selectRect;
			var Image = editor.Image;
			var editService = editor.EditService;

			var left = Canvas.GetLeft(selectRect);
			var top = Canvas.GetTop(selectRect);
			var rect = new Rectangle(
				(int)Math.Round(left),
				(int)Math.Round(top),
				(int)Math.Round(selectRect.Width),
				(int)Math.Round(selectRect.Height));
			Image.EraseImage(rect);
			var image = Image.Copy();
			if (editService.CanEdit((image, "Delete rectangle region"))) {
				editService.Edit((image, "Delete rectangle region"));
			}
			editor.EdittingState = NotEditting;
		}
	}
}