namespace PlotDigitizer.WPF
{
	public abstract class EdittingState : EditorState
	{
		public static NotEditting NotEditting { get; } = new NotEditting();
		public static Drawing Drawing { get; } = new Drawing();
		public static Erasing Erasing { get; } = new Erasing();
		public static PolySelected PolySelected { get; } = new PolySelected();
		public static PolySelecting PolySelecting { get; } = new PolySelecting();
		public static RectSelected RectSelected { get; } = new RectSelected();
		public static RectSelecting RectSelecting { get; } = new RectSelecting();
	}
}