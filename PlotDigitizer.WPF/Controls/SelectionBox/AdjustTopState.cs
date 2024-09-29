using PlotDigitizer.Core;
using System.Windows.Input;

namespace PlotDigitizer.WPF
{
    public class AdjustTopState : SelectionBoxState
    {
        public override void MouseMove(SelectionBox box, MouseEventArgs e)
        {
            base.MouseMove(box, e);
            var mousePos = e.GetPosition(box);
            var bottom = box.BoxBottom;
            box.BoxTop = MathHelpers.Clamp(mousePos.Y, bottom - 1, 0);
            box.BoxHeight = bottom - box.BoxTop;
        }
    }
}
