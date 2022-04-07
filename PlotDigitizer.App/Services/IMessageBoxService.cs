using System;
using System.Collections.Generic;
using System.Text;

namespace PlotDigitizer.App
{
	public interface IMessageBoxService
	{
		void Show_OK(string message, string caption);
		bool Show_OkCancel(string message, string caption);
		bool Show_Warning_OkCancel(string message, string caption);
	}
}
