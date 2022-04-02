using Emgu.CV;
using Emgu.CV.Structure;
using Microsoft.Win32;
using PlotDigitizer.Core;
using PropertyChanged;
using System;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace PlotDigitizer.App
{
	[AddINotifyPropertyChangedInterface]
	public partial class LoadPage : Page
	{
		private readonly Model model;
		private bool isDropFile;
		private bool isDropUrl;
		private bool isDropEnabled;

		public BitmapSource ImageSource { get; private set; }

		public LoadPage()
		{
			InitializeComponent();
#if DEBUG
			Loaded += (s, e) => imageControl.Visibility = Visibility.Visible;
#endif
			Loaded += LoadPage_Loaded;
			Unloaded += LoadPage_Unloaded;
		}


		private void LoadPage_Loaded(object sender, RoutedEventArgs e)
		{
			ImageSource = model?.InputImage?.ToBitmapSource();
		}
		private void LoadPage_Unloaded(object sender, RoutedEventArgs e)
		{
			model.InputImage = ImageSource?.ToBitmap().ToImage<Rgba, byte>();
		}

		public LoadPage(Model model) : this()
		{
			this.model = model;
		}

		public event EventHandler NextPage;
		private void OnNextPage()
		{
			NextPage?.Invoke(this, null);
		}
		private void SetModelImage(BitmapSource source)
		{
			ImageSource = source;
			OnNextPage();
		}

		private void BrowseButton_Loaded(object sender, RoutedEventArgs e)
		{
			(sender as UIElement).Focus();
		}

		private void BrowseButton_Click(object sender, RoutedEventArgs e)
		{
			var dialog = new OpenFileDialog
			{
				Filter = "All |*.jpg;*.jpeg;*.png;*.bmp;*.tif|" +
				"(*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
				"(*.png)|*.png|" +
				"(*.bmp)|*.bmp|" +
				"(*.tif)|*.tif|" +
				"Any |*.*"
			};
			if (dialog.ShowDialog() != true) {
				return;
			}
			SetModelImage(LoadImage(dialog.FileName));
		}

		private void PasteButton_Click(object sender, RoutedEventArgs e)
		{
			PasteImage();
		}

		private void Paste_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			PasteImage();
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
				SetModelImage(LoadImage(filename));
			} else if (isDropUrl) {
				var uri = new Uri(e.Data.GetData(DataFormats.Text).ToString(), UriKind.Absolute);
				SetModelImage(new BitmapImage(uri));
			}
		}

		private void PasteImage()
		{
			if (Clipboard.ContainsImage()) {
				SetModelImage(Clipboard.GetImage());
			} else if (Clipboard.ContainsFileDropList()) {
				SetModelImage(LoadImage(Clipboard.GetFileDropList()[0]));
			} else {
				MessageBox.Show("Clipboard does not contain image.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
				return;
			}
		}

		private BitmapSource LoadImage(string filename)
		{
			if (!File.Exists(filename)) {
				MessageBox.Show("Input file is not valid.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
				return null;
			}
			try {
				return new BitmapImage(new Uri(filename));
			}
			catch (Exception ex) {
				MessageBox.Show(ex.Message, "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
				return null;
			}
		}

		
	}
}