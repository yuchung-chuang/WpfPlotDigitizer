using System;
using System.Collections.Generic;
using System.Text;

namespace PlotDigitizer.App
{
	public interface IFileDialogService
	{
		FileDialogResults OpenFileDialog();
		FileDialogResults SaveFileDialog();
	}
}
