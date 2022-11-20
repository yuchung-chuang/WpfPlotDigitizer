using Microsoft.Xaml.Behaviors;

using System.Windows.Controls;
using System.Windows.Input;

namespace PlotDigitizer.App
{
	public class SelectAllTextOnFocusBehavior : Behavior<TextBox>
	{
		protected override void OnAttached()
		{
			base.OnAttached();
			AssociatedObject.GotKeyboardFocus += GotKeyboardFocus;
			AssociatedObject.GotMouseCapture += GotMouseCapture;
			AssociatedObject.PreviewMouseLeftButtonDown += PreviewMouseLeftButtonDown;
		}

		protected override void OnDetaching()
		{
			base.OnDetaching();
			AssociatedObject.GotKeyboardFocus -= GotKeyboardFocus;
			AssociatedObject.GotMouseCapture -= GotMouseCapture;
			AssociatedObject.PreviewMouseLeftButtonDown -= PreviewMouseLeftButtonDown;
		}

		private void GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
		{
			AssociatedObject.SelectAll();
		}

		private void GotMouseCapture(object sender, MouseEventArgs e)
		{
			AssociatedObject.SelectAll();
		}

		private void PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			if (!AssociatedObject.IsKeyboardFocusWithin) {
				AssociatedObject.Focus();
				e.Handled = true;
			}
		}
	}
}
