using Emgu.CV;
using Emgu.CV.Structure;
using System.Collections.Specialized;

namespace PlotDigitizer.Core
{
	public interface IClipboardService
	{
		bool ContainsFileDropList();
		bool ContainsImage();
		StringCollection GetFileDropList();
		Image<Rgba, byte> GetImage();
	}
}
