using Emgu.CV;
using Emgu.CV.Structure;
using PlotDigitizer.Core;
using System;

namespace PlotDigitizer.App
{
	public class LoadPageViewModel : ViewModelBase
	{
		private readonly IFileDialogService fileDialogService;

		public event EventHandler NextPage;

		public RelayCommand BrowseCommand { get; private set; }

		public Model Model { get; }

		public LoadPageViewModel()
		{
			BrowseCommand = new RelayCommand(Browse);
		}


		public LoadPageViewModel(Model model, IFileDialogService fileDialogService) : this()
		{
			Model = model;
			this.fileDialogService = fileDialogService;
		}
		

		private void OnNextPage()
		{
			NextPage?.Invoke(this, null);
		}

		public virtual void RaiseNextPage()
		{
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
			Model.InputImage = new Image<Rgba, byte>(result.FileName);
			OnNextPage();
		}

	}
}
