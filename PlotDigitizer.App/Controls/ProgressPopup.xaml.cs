using System;
using System.Windows;

namespace PlotDigitizer.App
{
	/// <summary>
	/// Interaction logic for WaitWindow.xaml
	/// </summary>
	public partial class ProgressPopup : Window
	{
		public event EventHandler Canceled;

		public ProgressPopup()
		{
			InitializeComponent();
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			Canceled?.Invoke(this, new EventArgs());
			Close();
		}
	}
}