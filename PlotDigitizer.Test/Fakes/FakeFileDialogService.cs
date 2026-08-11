using System.Collections.Generic;

namespace PlotDigitizer.Core.Tests.Fakes
{
	/// <summary>
	/// Hand-rolled fake for <see cref="IFileDialogService"/>. Returns a canned
	/// <see cref="FileDialogResults"/> and records the filters it was asked for.
	/// </summary>
	internal sealed class FakeFileDialogService : IFileDialogService
	{
		public FileDialogResults OpenResult { get; set; } = new FileDialogResults(null, false);

		public FileDialogResults SaveResult { get; set; } = new FileDialogResults(null, false);

		public int OpenCallCount { get; private set; }

		public int SaveCallCount { get; private set; }

		public List<string> RequestedFilters { get; } = [];

		public List<string> RequestedFileNames { get; } = [];

		public FileDialogResults OpenFileDialog(string filter = null, string filename = null)
		{
			OpenCallCount++;
			RequestedFilters.Add(filter);
			RequestedFileNames.Add(filename);
			return OpenResult;
		}

		public FileDialogResults SaveFileDialog(string filter = null, string filename = null)
		{
			SaveCallCount++;
			RequestedFilters.Add(filter);
			RequestedFileNames.Add(filename);
			return SaveResult;
		}
	}
}
