using System.Windows.Input;

namespace PlotDigitizer.WPF
{
    public abstract class SelectionBoxState
    {
        public virtual void MouseDown(SelectionBox box, MouseButtonEventArgs e) { }
        public virtual void MouseMove(SelectionBox box, MouseEventArgs e) { }
        public virtual void MouseUp(SelectionBox box, MouseButtonEventArgs e)
        {
            box.ReleaseMouseCapture();
            box.State = SelectionBoxStates.None;
        }
    }
}
