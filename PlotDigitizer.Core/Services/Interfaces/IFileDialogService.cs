namespace PlotDigitizer.Core
{
	public interface IFileDialogService
	{
		FileDialogResults OpenFileDialog(string filter = null, string filename = null);

		FileDialogResults SaveFileDialog(string filter = null, string filename = null);
	}

	public class FileDialogResults(string fileName, bool hasFile)
	{
		public string FileName { get; set; } = fileName;

		public bool IsValid { get; set; } = hasFile;
	}
}