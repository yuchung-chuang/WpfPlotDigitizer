using Emgu.CV.Util;

using PlotDigitizer.Core;

using System;
using System.Drawing;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;

namespace PlotDigitizer.App
{
	public class PolySelected : EdittingState
	{
		public override void MouseDown(Editor editor, MouseButtonEventArgs e)
		{
			base.MouseDown(editor, e);
			editor.EdittingState = NotEditting;
			EditorMode.PolyMode.MouseDown(editor, e);
		}

		public override void KeyDown(Editor editor, KeyEventArgs e)
		{
			base.KeyDown(editor, e);
			var Image = editor.Image;
			var editService = editor.EditService;
			var selectPoly = editor.selectPoly;

			// erase pixels within poly
			var points = new VectorOfPoint(selectPoly.Points.Select(p =>
			{
				return new Point((int)Math.Round(p.X), (int)Math.Round(p.Y));
			}).ToArray());
			Image.EraseImage(points);
			// execute edit command
			var image = Image.Copy();
			if (editService.CanEdit((image, "Delete polygon region"))) {
				editService.Edit((image, "Delete polygon region"));
			}

			editor.EdittingState = NotEditting;
		}
	}
}