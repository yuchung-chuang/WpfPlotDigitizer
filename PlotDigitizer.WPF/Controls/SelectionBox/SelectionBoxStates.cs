namespace PlotDigitizer.WPF
{
    public abstract class SelectionBoxStates
    {
        public static readonly NullState None = new();
        public static readonly AdjustLeftState AdjustLeft = new();
        public static readonly AdjustRightState AdjustRight = new();
        public static readonly AdjustTopState AdjustTop = new();
        public static readonly AdjustBottomState AdjustBottom = new();
    }
}
