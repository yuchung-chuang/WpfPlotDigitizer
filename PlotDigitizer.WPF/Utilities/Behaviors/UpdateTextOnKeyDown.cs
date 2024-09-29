using Microsoft.Xaml.Behaviors;
using System.Windows.Controls;
using System.Windows.Input;

namespace PlotDigitizer.WPF
{
    public class UpdateTextOnKeyDown : Behavior<TextBox>
    {
        public Key Key { get; set; } = Key.Enter;

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.KeyDown += AssociatedObject_KeyDown;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.KeyDown -= AssociatedObject_KeyDown;
        }

        private void AssociatedObject_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key) {
                return;
            }
            AssociatedObject.GetBindingExpression(TextBox.TextProperty).UpdateSource();
        }
    }
}