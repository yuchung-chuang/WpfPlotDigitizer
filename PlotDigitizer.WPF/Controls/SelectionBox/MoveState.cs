using System.Windows;
using System.Windows.Input;

namespace PlotDigitizer.WPF
{
    public class MoveState(Point boxAnchor, Point mouseAnchor) : SelectionBoxState
    {
        public static readonly MoveState Empty = new(default, default);

        public override void MouseMove(SelectionBox box, MouseEventArgs e)
        {
            base.MouseMove(box, e);

            var mousePos = e.GetPosition(box);

            var dx = mousePos.X - mouseAnchor.X;
            var dy = mousePos.Y - mouseAnchor.Y;
            box.BoxLeft = boxAnchor.X + dx;
            box.BoxTop = boxAnchor.Y + dy;
        }
    }
}
