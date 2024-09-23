using System.Diagnostics;
using System.Windows.Input;

namespace PlotDigitizer.WPF
{
	public class PencilMode : EditorMode
	{
		public Stopwatch Stopwatch { get; } = new Stopwatch();

		public override void MouseDown(Editor editor, MouseButtonEventArgs e)
		{
			if (editor.EdittingState != EdittingState.NotEditting) {
				return;
			}
			base.MouseDown(editor, e);
			editor.mainGrid.CaptureMouse();
			Stopwatch.Restart();
			editor.EdittingState = EdittingState.Drawing;
			EdittingState.Drawing.MouseMove(editor, e);
		}
	}
}