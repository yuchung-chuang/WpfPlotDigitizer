using Emgu.CV;
using Emgu.CV.Structure;
using PlotDigitizer.Core;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace PlotDigitizer.App
{
	public partial class LoadPage : Page
	{
		private LoadPageViewModel viewModel;
		private bool isDropFile;
		private bool isDropUrl;
		private bool isDropEnabled;

		public LoadPage()
		{
			InitializeComponent();
#if DEBUG
			Loaded += (s, e) => imageControl.Visibility = Visibility.Visible;
#endif
			Loaded += LoadPage_Loaded;
		}

		private void LoadPage_Loaded(object sender, RoutedEventArgs e)
		{
			viewModel = DataContext as LoadPageViewModel;
		}

		private void BrowseButton_Loaded(object sender, RoutedEventArgs e)
		{
			(sender as UIElement).Focus();
		}

		private void Paste_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			if (viewModel.PasteCommand.CanExecute()) {
				viewModel.PasteCommand.Execute();
			}
		}

		private void Page_DragOver(object sender, DragEventArgs e)
		{
			isDropFile = e.Data.GetDataPresent(DataFormats.FileDrop)
				&& File.Exists((e.Data.GetData(DataFormats.FileDrop) as string[])[0]);
			isDropUrl = e.Data.GetDataPresent(DataFormats.Text)
				// check if it's valid Uri
				&& Uri.TryCreate(e.Data.GetData(DataFormats.Text).ToString(), UriKind.Absolute, out var uri)
				// check if it's a web uri
				&& (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
			isDropEnabled = isDropFile || isDropUrl;

			e.Effects = isDropEnabled ? DragDropEffects.Copy : DragDropEffects.None;
			if (!isDropEnabled) {
				e.Handled = true;
			}
		}

		private void Page_Drop(object sender, DragEventArgs e)
		{
			if (isDropFile) {
				var filename = (e.Data.GetData(DataFormats.FileDrop) as string[])[0];
				viewModel.SetModelImage(new Image<Rgba, byte>(filename));
			} else if (isDropUrl) {
				var uri = new Uri(e.Data.GetData(DataFormats.Text).ToString(), UriKind.Absolute);

				var popup = new ProgressPopup
				{
					Owner = Application.Current.MainWindow,
					IsIndeterminate = false
				};

				var bitmapImage = new BitmapImage();
				bitmapImage.BeginInit();
				bitmapImage.UriSource = uri;
				bitmapImage.DownloadProgress += (s, e) =>
				{
					popup.Value = e.Progress;
				};
				bitmapImage.DownloadCompleted += (s, e) =>
				{
					viewModel.SetModelImage(bitmapImage.ToBitmap().ToImage<Rgba, byte>());
					popup.Close();
				};
				bitmapImage.DownloadFailed += (s, e) =>
				{
					MessageBox.Show(e.ErrorException.Message);
					popup.Close();
				};
				bitmapImage.EndInit();

				popup.Show();
			}
		}
	}
}