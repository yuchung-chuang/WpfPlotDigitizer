using PlotDigitizer.Core;
using System.Windows.Input;

namespace PlotDigitizer.WPF
{
    public class AdjustBottomState : SelectionBoxState
    {
        public override void MouseMove(SelectionBox box, MouseEventArgs e)
        {
            base.MouseMove(box, e);
            var mousePos = e.GetPosition(box);
            box.BoxHeight = MathHelpers.Clamp(mousePos.Y - box.BoxTop, box.ActualHeight - box.BoxTop, 1);
        }
    }
}
