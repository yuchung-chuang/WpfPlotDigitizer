using Emgu.CV;
using Emgu.CV.Structure;
using PlotDigitizer.Core;
using System;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

namespace PlotDigitizer.App
{
	public class LoadPageViewModel : ViewModelBase
	{
		private readonly Model model;

		public event EventHandler NextPage;

		public BitmapSource ImageSource => model?.InputImage?.ToBitmapSource();

		public RelayCommand PasteCommand { get; private set; }
		public LoadPageViewModel()
		{
			PasteCommand = new RelayCommand(PasteImage);
		}


		public LoadPageViewModel(Model model) : this()
		{
			this.model = model;
			model.PropertyChanged += Model_PropertyChanged;
		}
		public void SetModelImage(BitmapSource source)
		{
			model.InputImage = source.ToBitmap().ToImage<Rgba, byte>();
			OnNextPage();
		}

		public BitmapSource LoadImage(string filename)
		{
			try {
				return new BitmapImage(new Uri(filename));
			}
			catch (Exception ex) {
				OnMessageBoxRequested(ex.Message, "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
				return null;
			}
		}
		private void PasteImage()
		{
			if (Clipboard.ContainsImage()) {
				SetModelImage(Clipboard.GetImage());
			} else if (Clipboard.ContainsFileDropList()) {
				SetModelImage(LoadImage(Clipboard.GetFileDropList()[0]));
			} else {
				OnMessageBoxRequested("Clipboard does not contain image.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
				return;
			}
		}

		private void Model_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == nameof(model.InputImage)) {
				OnPropertyChanged(nameof(ImageSource));
			}
		}

		private void OnNextPage()
		{
			NextPage?.Invoke(this, null);
		}

	}
}
