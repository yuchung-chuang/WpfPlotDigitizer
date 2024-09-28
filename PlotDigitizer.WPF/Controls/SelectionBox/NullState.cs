using System.Windows.Input;

namespace PlotDigitizer.WPF
{
    public class NullState : SelectionBoxState
    {
        public override void MouseDown(SelectionBox box, MouseButtonEventArgs e)
        {
            base.MouseDown(box, e);
            if (e.ClickCount >= 2) {
                var x = 1;
            }
            if (!box.Gesture.Matches(box, e))
                return;
            var mousePos = e.GetPosition(box);
            var state = box.GetState(mousePos);
            box.UpdateCursor(state);

            if (state is not NullState) {
                box.State = state;
                box.CaptureMouse();

                e.Handled = true; // block other events
            }
        }

        public override void MouseMove(SelectionBox box, MouseEventArgs e)
        {
            base.MouseMove(box, e);
            var mousePos = e.GetPosition(box);
            var state = box.GetState(mousePos);
            box.UpdateCursor(state);
        }
    }
}
