using System.Diagnostics;
using System.Windows.Input;

namespace PlotDigitizer.App
{
	public class PencilMode : EditorState
	{
		public static PencilMode Instance { get; } = new PencilMode();
		public Stopwatch Stopwatch { get; } = new Stopwatch();
		public override void MouseDown(Editor editor, MouseButtonEventArgs e)
		{
			if (editor.EdittingState != NotEditting.Instance) {
				return;
			}
			base.MouseDown(editor, e);
			editor.mainGrid.CaptureMouse();
			Stopwatch.Restart();
			editor.EdittingState = Drawing.Instance;
			Drawing.Instance.MouseMove(editor, e);
		}
	}
}