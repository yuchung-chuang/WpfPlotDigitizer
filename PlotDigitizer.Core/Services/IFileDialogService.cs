namespace PlotDigitizer.Core
{
	public interface IFileDialogService
	{
		FileDialogResults OpenFileDialog(string filter = null, string filename = null);
		FileDialogResults SaveFileDialog(string filter = null, string filename = null);
	}
}
