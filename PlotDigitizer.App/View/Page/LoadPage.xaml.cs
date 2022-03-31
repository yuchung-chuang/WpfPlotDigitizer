using Emgu.CV;
using Emgu.CV.Structure;
using Microsoft.Win32;
using PlotDigitizer.Core;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace PlotDigitizer.App
{
	/// <summary>
	/// Interaction logic for LoadPage.xaml
	/// </summary>
	public partial class LoadPage : Page, INotifyPropertyChanged
	{
		private readonly Model model;
		private bool isDropFile;
		private bool isDropUrl;
		private bool isDropEnabled;

		public event PropertyChangedEventHandler PropertyChanged;

		public BitmapSource ImageSource { get; private set; }

		public LoadPage()
		{
			InitializeComponent();
			Loaded += LoadPage_Loaded;
			Unloaded += LoadPage_Unloaded;
		}

		public LoadPage(Model model) : this()
		{
			this.model = model;
		}

		private void LoadPage_Loaded(object sender, RoutedEventArgs e)
		{
#if DEBUG
			imageControl.Visibility = Visibility.Visible;
#endif
		}

		private void LoadPage_Unloaded(object sender, RoutedEventArgs e)
		{
			model.InputImage = ImageSource?.ToBitmap().ToImage<Rgba, byte>();
		}

		private void browseButton_Loaded(object sender, RoutedEventArgs e)
		{
			(sender as UIElement).Focus();
		}

		private void browseButton_Click(object sender, RoutedEventArgs e)
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
			ImageSource = loadImage(dialog.FileName);
		}

		private void pasteButton_Click(object sender, RoutedEventArgs e)
		{
			pasteImage();
		}

		private void Paste_Executed(object sender, ExecutedRoutedEventArgs e)
		{
			pasteImage();
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

			if (isDropEnabled) {
				e.Effects = DragDropEffects.Copy;
			} else {
				e.Effects = DragDropEffects.None;
				e.Handled = true;
			}
		}

		private void Page_Drop(object sender, DragEventArgs e)
		{
			if (isDropFile) {
				var filename = (e.Data.GetData(DataFormats.FileDrop) as string[])[0];
				ImageSource = loadImage(filename);
			} else if (isDropUrl) {
				var uri = new Uri(e.Data.GetData(DataFormats.Text).ToString(), UriKind.Absolute);
				ImageSource = new BitmapImage(uri);
			}
		}

		private void pasteImage()
		{
			if (Clipboard.ContainsImage()) {
				ImageSource = Clipboard.GetImage();
			} else if (Clipboard.ContainsFileDropList()) {
				ImageSource = loadImage(Clipboard.GetFileDropList()[0]);
			} else {
				MessageBox.Show("Clipboard does not contain image.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
				return;
			}
		}

		private BitmapSource loadImage(string filename)
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