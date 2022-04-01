using Emgu.CV.Util;
using PlotDigitizer.Core;
using System;
using System.Drawing;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace PlotDigitizer.App
{
	public abstract class EdittingState : State
	{
		
	}

	public class NotEditting : EdittingState 
	{
		public static NotEditting Instance { get; } = new NotEditting();
	}

	public class PolySelected : EdittingState
	{
		public static PolySelected Instance { get; } = new PolySelected();
		public override void MouseDown(Editor editor, MouseButtonEventArgs e)
		{
			base.MouseDown(editor, e);
			editor.EdittingState = NotEditting.Instance;
			PolyMode.Instance.MouseDown(editor, e);
		}

		public override void KeyDown(Editor editor, KeyEventArgs e)
		{
			base.KeyDown(editor, e);
			var Image = editor.Image;
			var EditManager = editor.EditManager;
			var selectPoly = editor.selectPoly;

			// erase pixels within poly
			var points = new VectorOfPoint(selectPoly.Points.Select(p =>
			{
				return new System.Drawing.Point((int)Math.Round(p.X), (int)Math.Round(p.Y));
			}).ToArray());
			Methods.EraseImage(Image, points);
			// execute edit command
			var image = Image.Copy();
			if (EditManager.EditCommand.CanExecute((image, "Delete polygon region"))) {
				EditManager.EditCommand.Execute((image, "Delete polygon region"));
			}

			editor.EdittingState = NotEditting.Instance;
		}
	}

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

	public class RectSelected : EdittingState
	{
		public static RectSelected Instance { get; } = new RectSelected();
		public override void MouseDown(Editor editor, MouseButtonEventArgs e)
		{
			base.MouseDown(editor, e);
			editor.EdittingState = NotEditting.Instance;
			RectMode.Instance.MouseDown(editor, e);
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
			editor.EdittingState = NotEditting.Instance;
		}
	}

	public class RectSelecting : EdittingState
	{
		public static RectSelecting Instance { get; } = new RectSelecting();
		public override void MouseMove(Editor editor, MouseEventArgs e)
		{
			base.MouseMove(editor, e);
			var selectRect = editor.selectRect;
			var Image = editor.Image;
			var mouseDownPos = RectMode.Instance.MouseDownPos;
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
			editor.EdittingState = RectSelected.Instance;
		}
	}

	public class Erasing : EdittingState
	{
		private static readonly int fps = 24;
		public static Erasing Instance { get; } = new Erasing();
		public override void MouseMove(Editor editor, MouseEventArgs e)
		{
			base.MouseMove(editor, e);
			var stopwatch = ErasorMode.Instance.Stopwatch;

			var centre = e.GetPosition(editor.editCanvas);
			var size = new Vector(editor.eraserRect.Width, editor.eraserRect.Height);
			var position = centre - size / 2;
			Canvas.SetLeft(editor.eraserRect, position.X);
			Canvas.SetTop(editor.eraserRect, position.Y);
			var rect = new Rectangle(
				(int)Math.Round(position.X),
				(int)Math.Round(position.Y),
				(int)Math.Round(size.X),
				(int)Math.Round(size.Y));
			Methods.EraseImage(editor.Image, rect);
			// update the image by "N" frames per second
			if (stopwatch.ElapsedMilliseconds > 1000 / fps) {
				editor.UpdateImageSource();
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

			editor.EdittingState = NotEditting.Instance;
		}
	}

	public class Drawing : EdittingState
	{
		private static readonly int fps = 24;
		public static Drawing Instance { get; } = new Drawing();
		public override void MouseMove(Editor editor, MouseEventArgs e)
		{
			base.MouseMove(editor, e);
			var stopwatch = PencilMode.Instance.Stopwatch;

			var centre = e.GetPosition(editor.editCanvas);
			var size = new Vector(editor.pencilPointer.Width, editor.pencilPointer.Height);
			var position = centre - size / 2;
			Canvas.SetLeft(editor.pencilPointer, position.X);
			Canvas.SetTop(editor.pencilPointer, position.Y);

			var centre2 = new System.Drawing.Point(
				(int)Math.Round(centre.X),
				(int)Math.Round(centre.Y));

			Methods.DrawImage(editor.Image, centre2, (int)size.X / 2);

			// update the image by "N" frames per second
			if (stopwatch.ElapsedMilliseconds > 1000 / fps) {
				editor.UpdateImageSource();
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