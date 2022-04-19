using System.Windows.Input;
using System.Windows.Media;

namespace PlotDigitizer.App
{
	public class PolyMode : EditorState
	{
		public static PolyMode Instance { get; } = new PolyMode();
		public override void MouseDown(Editor editor, MouseButtonEventArgs e)
		{
			if (editor.EdittingState != NotEditting.Instance) {
				return;
			}
			base.MouseDown(editor, e);
			var position = e.GetPosition(editor.editCanvas);
			// add the second point as an indicator
			editor.selectPoly.Points = new PointCollection { position, position };
			editor.EdittingState = PolySelecting.Instance;
		}
	}
}