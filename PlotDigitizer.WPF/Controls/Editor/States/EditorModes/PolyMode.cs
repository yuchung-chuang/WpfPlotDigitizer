using System.Windows.Input;
using System.Windows.Media;

namespace PlotDigitizer.WPF
{
	public class PolyMode : EditorMode
	{
		public override void MouseDown(Editor editor, MouseButtonEventArgs e)
		{
			if (editor.EdittingState != EdittingState.NotEditting) {
				return;
			}
			base.MouseDown(editor, e);
			var position = e.GetPosition(editor.editCanvas);
			// add the second point as an indicator
			editor.selectPoly.Points = new PointCollection { position, position };
			editor.EdittingState = EdittingState.PolySelecting;
		}
	}
}