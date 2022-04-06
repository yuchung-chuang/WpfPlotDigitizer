using Emgu.CV;
using Emgu.CV.Structure;
using PlotDigitizer.Core;
using System;
using System.Windows;
using System.Windows.Media.Imaging;

namespace PlotDigitizer.App
{
	public class LoadPageViewModel : ViewModelBase
	{
		public event EventHandler NextPage;

		public RelayCommand PasteCommand { get; private set; }

		public Model Model { get; }

		public LoadPageViewModel()
		{
			PasteCommand = new RelayCommand(PasteImage);
		}


		public LoadPageViewModel(Model model) : this()
		{
			Model = model;
		}
		public void SetModelImage(BitmapSource source)
		{
			Model.InputImage = source.ToBitmap().ToImage<Rgba, byte>();
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

		private void OnNextPage()
		{
			NextPage?.Invoke(this, null);
		}

	}
}
