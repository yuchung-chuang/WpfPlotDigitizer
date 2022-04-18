using Microsoft.Win32;
using PlotDigitizer.Core;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows;
using System.Windows.Navigation;
using System.Xml;
using System.Xml.Serialization;

namespace PlotDigitizer.App
{
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
		}

		private void mainFrame_Navigating(object sender, NavigatingCancelEventArgs e)
		{
			// disable frame navigation to prevent keyboard input conflict
			if (e.NavigationMode == NavigationMode.Back ||
				e.NavigationMode == NavigationMode.Forward) {
				e.Cancel = true;
			}
		}

		protected override void OnClosed(EventArgs e)
		{
			base.OnClosed(e);

			Application.Current.Shutdown(0);
		}
	}
}
