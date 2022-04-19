using Emgu.CV;
using Emgu.CV.Structure;
using System;

namespace PlotDigitizer.Core
{
	public class LoadPageViewModel : PageViewModelBase
	{
		private readonly IFileDialogService fileDialogService;
		private readonly IClipboard clipboard;
		private readonly IMessageBoxService messageBox;

		public event EventHandler NextPage;

		public RelayCommand BrowseCommand { get; private set; }
		public RelayCommand PasteCommand { get; private set; }

		public Model Model { get; }

		public LoadPageViewModel()
		{
			Name = "LoadPage";
			BrowseCommand = new RelayCommand(Browse);
			PasteCommand = new RelayCommand(Paste);
		}

		public LoadPageViewModel(Model model, 
			IFileDialogService fileDialogService,
			IClipboard clipboard,
			IMessageBoxService messageBox) : this()
		{
			Model = model;
			this.fileDialogService = fileDialogService;
			this.clipboard = clipboard;
			this.messageBox = messageBox;
		}
		
		private void OnNextPage()
		{
			NextPage?.Invoke(this, null);
		}

		private void Paste()
		{
			if (clipboard.ContainsImage()) {
				SetModelImage(clipboard.GetImage());
			} else if (clipboard.ContainsFileDropList()) {
				SetModelImage(new Image<Rgba, byte>(clipboard.GetFileDropList()[0]));
			} else {
				messageBox.Show_OK("Clipboard does not contain image.", "Warning");
				return;
			}
		}

		public void SetModelImage(Image<Rgba, byte> image)
		{
			Model.InputImage = image;
			OnNextPage();
		}

		private void Browse()
		{
			var filter = "Images (*.jpg;*.jpeg;*.png;*.bmp;*.tif) |*.jpg;*.jpeg;*.png;*.bmp;*.tif|" +
				"(*.jpg;*.jpeg) |*.jpg;*.jpeg|" +
				"(*.png) |*.png|" +
				"(*.bmp) |*.bmp|" +
				"(*.tif) |*.tif|" +
				"Any |*.*";
			var result = fileDialogService.OpenFileDialog(filter);
			if (!result.IsValid) {
				return;
			}
			SetModelImage(new Image<Rgba, byte>(result.FileName));
		}

	}
}
