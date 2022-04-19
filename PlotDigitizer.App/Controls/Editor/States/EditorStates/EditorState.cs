namespace PlotDigitizer.App
{
	public abstract class EditorState : State
	{
		public override void Enter(Editor editor)
		{
			editor.EdittingState = NotEditting.Instance;
		}
	}
}