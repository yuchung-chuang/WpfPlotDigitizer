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
		private readonly IMessageBoxService messageBoxService;
		private readonly IFileDialogService fileDialogService;

		public event EventHandler NextPage;

		public RelayCommand PasteCommand { get; private set; }
		public RelayCommand BrowseCommand { get; private set; }

		public Model Model { get; }

		public LoadPageViewModel()
		{
			PasteCommand = new RelayCommand(PasteImage);
			BrowseCommand = new RelayCommand(Browse);
		}


		public LoadPageViewModel(Model model, IMessageBoxService messageBoxService, IFileDialogService fileDialogService) : this()
		{
			Model = model;
			this.messageBoxService = messageBoxService;
			this.fileDialogService = fileDialogService;
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
				messageBoxService.Show_OK(ex.Message, "Warning");
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
				messageBoxService.Show_OK("Clipboard does not contain image.", "Warning");
				return;
			}
		}

		private void OnNextPage()
		{
			NextPage?.Invoke(this, null);
		}

		private void Browse()
		{
			var result = fileDialogService.OpenFileDialog();
			if (!result.IsValid) {
				return;
			}
			var image = LoadImage(result.FileName);
			SetModelImage(image);
		}

	}
}
