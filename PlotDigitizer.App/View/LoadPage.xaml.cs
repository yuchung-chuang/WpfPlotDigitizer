using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace PlotDigitizer.App
{
	public partial class LoadPage : UserControl
	{
		public LoadPage()
		{
			InitializeComponent();
		}

		private void Page_DragOver(object sender, DragEventArgs e)
		{
			var isDropFile = e.Data.GetDataPresent(DataFormats.FileDrop)
				&& File.Exists((e.Data.GetData(DataFormats.FileDrop) as string[])[0]);
			var isDropUrl = e.Data.GetDataPresent(DataFormats.Text)
				// check if it's valid Uri
				&& Uri.TryCreate(e.Data.GetData(DataFormats.Text).ToString(), UriKind.Absolute, out var uri)
				// check if it's a web uri
				&& (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
			var isDropEnabled = isDropFile || isDropUrl;

			e.Effects = isDropEnabled ? DragDropEffects.Copy : DragDropEffects.None;
			if (!isDropEnabled) {
				e.Handled = true;
			}
		}
	}
}