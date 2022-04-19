using Microsoft.Xaml.Behaviors;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace PlotDigitizer.App
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

		private void AssociatedObject_Loaded(object sender, RoutedEventArgs e)
		{
			AssociatedObject.Focus();
		}

	}
}
