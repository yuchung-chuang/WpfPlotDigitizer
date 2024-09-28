using System.Windows.Input;

namespace PlotDigitizer.WPF
{
	public abstract class EditorState
	{
		public virtual void MouseDown(Editor editor, MouseButtonEventArgs e)
		{ }

		public virtual void MouseMove(Editor editor, MouseEventArgs e)
		{ }

		public virtual void MouseUp(Editor editor, MouseButtonEventArgs e)
		{ }

		public virtual void KeyDown(Editor editor, KeyEventArgs e)
		{ }

		public virtual void Enter(Editor editor)
		{ }
	}
}