using PlotDigitizer.Core;
using System.Windows.Input;

namespace PlotDigitizer.WPF
{
    public class AdjustRightState : SelectionBoxState
    {
        public override void MouseMove(SelectionBox box, MouseEventArgs e)
        {
            base.MouseMove(box, e);
            var mousePos = e.GetPosition(box);
            box.BoxWidth = MathHelpers.Clamp(mousePos.X - box.BoxLeft, box.ActualWidth - box.BoxLeft, 1);
        }
    }
}
