namespace PlotDigitizer.Core
{
	public class FileDialogResults(string fileName, bool hasFile)
	{
		public string FileName { get; set; } = fileName;

		public bool IsValid { get; set; } = hasFile;
	}
}