using System;
using System.Drawing;

namespace PlotDigitizer.Core
{
	public class DropEventArgs : EventArgs
	{
		public enum DropType
		{
			File,
			Url,
            Image
        }

		public DropType Type { get; set; }
		public string FileName { get; set; }
		public Uri Url { get; set; }
        public Bitmap Image { get; set; }
    }
}