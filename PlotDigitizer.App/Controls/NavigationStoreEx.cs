using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

using Wpf.Ui.Common;
using Wpf.Ui.Controls;

namespace PlotDigitizer.App
{
    public class NavigationStoreEx : NavigationStore
    {
		public event EventHandler PreviewNavigate;
		protected override void OnNavigationItemClicked(object sender, RoutedEventArgs e)
		{
			PreviewNavigate?.Invoke(this, null);
			base.OnNavigationItemClicked(sender, e);
		}
	}
}
