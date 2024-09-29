using System.Windows.Input;

namespace PlotDigitizer.WPF
{
    public class AdjustState : SelectionBoxState
    {
        public SelectionBoxState StateX { get; set; } = SelectionBoxStates.None;
        public SelectionBoxState StateY { get; set; } = SelectionBoxStates.None;
        public override void MouseMove(SelectionBox box, MouseEventArgs e)
        {
            base.MouseMove(box, e);
            StateX.MouseMove(box, e);
            StateY.MouseMove(box, e);
        }
    }
}
