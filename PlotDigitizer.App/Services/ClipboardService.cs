using Emgu.CV;
using Emgu.CV.Structure;

using PlotDigitizer.Core;

using System.Collections.Specialized;
using System.Windows;

namespace PlotDigitizer.App
{
	public class ClipboardService : IClipboardService
	{
		public bool ContainsImage() => Clipboard.ContainsImage();

		public bool ContainsFileDropList() => Clipboard.ContainsFileDropList();

		public Image<Rgba, byte> GetImage() => Clipboard.GetImage().ToBitmap().ToImage<Rgba, byte>();

		public StringCollection GetFileDropList() => Clipboard.GetFileDropList();
	}
}