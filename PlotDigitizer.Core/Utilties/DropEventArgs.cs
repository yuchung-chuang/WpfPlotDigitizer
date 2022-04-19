using System;

namespace PlotDigitizer.Core
{
	public class DropEventArgs : EventArgs
	{
		public enum DropType
		{
			File,
			Url
		}

		public DropType Type { get; set; }
		public string FileName { get; set; }
		public Uri Url { get; set; }
	}
}
