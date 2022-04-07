using Microsoft.Win32;

namespace PlotDigitizer.App
{
	public class FileDialogService : IFileDialogService
	{
		public FileDialogResults OpenFileDialog()
		{
			var dialog = new OpenFileDialog
			{
				Filter = "Images (*.jpg;*.jpeg;*.png;*.bmp;*.tif) |*.jpg;*.jpeg;*.png;*.bmp;*.tif|" +
				"(*.jpg;*.jpeg) |*.jpg;*.jpeg|" +
				"(*.png) |*.png|" +
				"(*.bmp) |*.bmp|" +
				"(*.tif) |*.tif|" +
				"Any |*.*"
			};

			var isValid = dialog.ShowDialog();
			return new FileDialogResults(dialog.FileName, isValid ?? false);
		}

		public FileDialogResults SaveFileDialog()
		{
			var dialog = new SaveFileDialog
			{
				Filter =
				"Comma-separated values file (*.csv) |*.csv|" +
				"Text file (*.txt) |*.txt|" +
				"Any (*.*) |*.*",
				FileName = "output",
			};
			var isValid = dialog.ShowDialog();
			return new FileDialogResults(dialog.FileName, isValid ?? false);
		}
	}
}
