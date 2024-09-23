using PropertyChanged;

using System;
using System.Windows;

namespace PlotDigitizer.WPF
{
	[AddINotifyPropertyChangedInterface]
	public partial class ProgressPopup : Window
	{
		public event EventHandler Canceled;

		public int Value { get; set; }

		public bool IsIndeterminate { get; set; } = true;

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