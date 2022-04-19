using System.Diagnostics;
using System.Windows.Input;

namespace PlotDigitizer.App
{
	public class EraserMode : EditorState
	{
		public static EraserMode Instance { get; } = new EraserMode();
		public Stopwatch Stopwatch { get; } = new Stopwatch();

		public override void MouseDown(Editor editor, MouseButtonEventArgs e)
		{
			if (editor.EdittingState != NotEditting.Instance) {
				return;
			}
			base.MouseDown(editor, e);
			editor.mainGrid.CaptureMouse();
			Stopwatch.Restart();
			editor.EdittingState = Erasing.Instance;
			Erasing.Instance.MouseMove(editor, e);
		}
	}
}