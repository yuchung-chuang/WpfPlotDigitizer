using Microsoft.Xaml.Behaviors;

using System.Windows;

namespace PlotDigitizer.WPF
{
	public class FocusOnLoaded : Behavior<FrameworkElement>
	{
		protected override void OnAttached()
		{
			base.OnAttached();
			AssociatedObject.Loaded += AssociatedObject_Loaded;
		}

		protected override void OnDetaching()
		{
			base.OnDetaching();
			AssociatedObject.Loaded -= AssociatedObject_Loaded;
		}

		private void AssociatedObject_Loaded(object sender, RoutedEventArgs e) => AssociatedObject.Focus();
	}
}