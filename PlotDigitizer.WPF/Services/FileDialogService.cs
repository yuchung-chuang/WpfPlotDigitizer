using Microsoft.Win32;

using PlotDigitizer.Core;

namespace PlotDigitizer.WPF
{
	public class FileDialogService : IFileDialogService
	{
		public FileDialogResults OpenFileDialog(string filter = null, string filename = null)
		{
			var dialog = new OpenFileDialog
			{
				Filter = filter,
				FileName = filename,
			};

			var isValid = dialog.ShowDialog();
			return new FileDialogResults(dialog.FileName, isValid ?? false);
		}

		public FileDialogResults SaveFileDialog(string filter = null, string filename = null)
		{
			var dialog = new SaveFileDialog
			{
				Filter = filter,
				FileName = filename,
			};
			var isValid = dialog.ShowDialog();
			return new FileDialogResults(dialog.FileName, isValid ?? false);
		}
	}
}