using PlotDigitizer.Core;
using System.Windows.Input;

namespace PlotDigitizer.WPF
{
    public class AdjustLeftState : SelectionBoxState
    {
        public override void MouseMove(SelectionBox box, MouseEventArgs e)
        {
            base.MouseMove(box, e);
            var mousePos = e.GetPosition(box);

            var right = box.BoxRight;
            box.BoxLeft = MathHelpers.Clamp(mousePos.X, right - 1, 0);
            box.BoxWidth = right - box.BoxLeft;
        }
    }
}
