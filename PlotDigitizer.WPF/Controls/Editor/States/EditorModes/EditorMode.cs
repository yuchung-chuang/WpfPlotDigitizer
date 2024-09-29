namespace PlotDigitizer.WPF
{
	public abstract class EditorMode : EditorState
	{
		public static NoMode NoMode { get; } = new NoMode();
		public static EraserMode EraserMode { get; } = new EraserMode();
		public static PencilMode PencilMode { get; } = new PencilMode();
		public static PolyMode PolyMode { get; } = new PolyMode();
		public static RectMode RectMode { get; } = new RectMode();

		public override void Enter(Editor editor) => editor.EdittingState = EdittingState.NotEditting;
	}
}