namespace PlotDigitizer.Core
{
	public class FileDialogResults
	{
		public FileDialogResults(string fileName, bool hasFile)
		{
			FileName = fileName;
			IsValid = hasFile;
		}

		public string FileName { get; set; }

		public bool IsValid { get; set; } = false;
	}
}